using BPServer.Core.MessageBus.Messages;
using BPServer.Core.Transports;
using BPServer.Transports;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BPServer.Autofac.Serial
{
    public class SerialPortTransport : ITransport, IDisposable
    {
        public bool IsRS485 { get; protected set; }
        private SerialPort stream;
        private bool isDisposed;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly ILogger log;
        private int delay => 200;

        public SerialPortTransport(IMessageFactory messageFactory, string serialPort, bool isRS485, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(serialPort))
            {
                throw new ArgumentException("message", nameof(serialPort));
            }

            MessageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            Name = serialPort;
            IsRS485 = isRS485;
            log = logger ?? throw new ArgumentNullException(nameof(logger));
            Init();
        }

        public bool Init()
        {
            if (!PortHelpers.PortNameExists(Name))
            {
                log.Error($"Port: '{Name}' not exists");
                //throw new Exception($"{port.PortName} not exists");
                return false;
            }
            stream = new SerialPort(Name)
            {
                BaudRate = 115200,
                ReadTimeout = 1000,
                WriteTimeout = 1000,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Encoding = Encoding.ASCII,
                NewLine = "\r\n"
            };
            stream.DataReceived += OnStreamDataReceived;
            stream.ErrorReceived += OnStreamErrorReceived;
            stream.PinChanged += OnStreamPinChanged;

            if (IsRS485)
            {
                stream.Handshake = Handshake.None;
                stream.RtsEnable = true;
                stream.DtrEnable = true;
                log.Verbose($"Port {Name} in RS485 mode");
            }

            stream.Open();
            if (!stream.IsOpen)
            {
                log.Error($"Error opening serial port {Name}");
                //throw new Exception($"Error opening serial port {port.PortName}");
                return false;
            }
            log.Information($"Port {Name} open");
            if (stream == null)
            {
                log.Error($"No serial port {Name}");
                // throw new Exception($"No serial port {port.PortName}");
                return false;
            }
            if (stream.CtsHolding)
            {
                log.Verbose($"Cts detected {Name}");
            }
            else
            {
                log.Verbose($"Cts NOT detected {Name}");
            }
            log.Information($"Port listener started: {Name}");
            return true;
        }

        private void OnStreamPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            log.Verbose($"On port: '{Name}' pin changed.");
        }

        private void OnStreamErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            log.Warning($"On port: '{Name}' ERROR recieved.");
        }

        private async void OnStreamDataReceived(object sender, SerialDataReceivedEventArgs e) => await ReadMessageAsync();
        

        private bool ReadHeader(Stopwatch sw, out ICollection<byte> list)
        {
            list = new List<byte>();
            do
            {
                if (sw.ElapsedMilliseconds >= TimeSpan.FromSeconds(1).TotalMilliseconds)
                {
                    log.Warning($"ReadTimeout header on '{Name}'");
                    return false;
                }
                byte _byte = 0;
                try
                {
                    _byte = (byte)stream.ReadByte();
                }
                catch (Exception e)
                {
                    log.Error($"{Name}: on head '{e.ToString()}'");
                    continue;
                }
                list.Add(_byte);
            } while (list.Count < 6);
            return true;
        }

        private bool ReadBody(Stopwatch sw, int length, out ICollection<byte> list)
        {
            int len = length;
            int k = len;

            list = new List<byte>();
            sw.Reset();
            do
            {
                if (sw.ElapsedMilliseconds >= TimeSpan.FromSeconds(1).TotalMilliseconds)
                {
                    log.Warning($"ReadTimeout body on '{Name}'");
                    return false;
                }
                byte _byte = 0;
                try
                {
                    _byte = (byte)stream.ReadByte();
                }
                catch(Exception e )
                {
                    log.Error($"{Name}: on body '{e.ToString()}'");
                    continue;
                }
                list.Add(_byte);
                k--;
            } while (stream.BytesToRead > 0 && 
                k > 0);
            return true;
        }

        private async Task ReadMessageAsync()
        {
            try
            {
                await semaphore.WaitAsync();
                Stopwatch sw = new Stopwatch();
                log.Debug($"Semaphore on '{Name}'");
                if (IsRS485 && stream.RtsEnable != true)
                {
                    stream.RtsEnable = true;
                    //await Task.Delay(TimeSpan.FromMilliseconds(50));
                }
                sw.Start();
                var bytes = new List<byte>();
                if (ReadHeader(sw, out ICollection<byte> list))
                {
                    bytes.AddRange(list);
                    if (ReadBody(sw, Message.HighLowToInt(
                        list.ElementAt(4),
                        list.ElementAt(5)
                        ), out ICollection<byte> body))
                    {
                        bytes.AddRange(body);

                        var res = bytes.ToArray();
                        string str = BitConverter.ToString(res);
                        log.Verbose($"{Name}: '{str}'");
                        if (MessageFactory.CreateMessage(res, out IMessage message))
                        {
                            OnDataReceived(message);
                        }
                        else
                        {
                            log.Warning($"{Name}: Message Factory failed create message");
                        }  
                    }
                    else
                    {
                        log.Warning($"Serial failed to read body, bytes:'{BitConverter.ToString(bytes.ToArray())}'");
                    }
                }
                else
                {
                    log.Warning($"Serial failed to read header, bytes:'{BitConverter.ToString(bytes.ToArray())}'");
                }
            }
            catch (IOException ex)
            {
                log.Error($"{Name}: '{ex.ToString()}'");
            }
            catch (System.TimeoutException ex)
            {
                log.Verbose($"{Name}: r:timeout");
            }
            catch (InvalidOperationException ex)
            {
                log.Error($"{Name}: '{ex.ToString()}'");
            }
            catch(Exception ex)
            {
                log.Error($"{Name}: '{ex.ToString()}'");
            }
            finally
            {
                semaphore.Release();
            }
        }

        private readonly IMessageFactory MessageFactory;
        public string Name { get; protected set; }

        public event EventHandler<IMessage> DataReceived;

        protected virtual void OnDataReceived(IMessage e)
        {
            EventHandler<IMessage> handler = DataReceived;
            handler?.Invoke(this, e);
        }

        public string GetInfo() => Name;

        public async Task PushDataAsync(IMessage input)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(delay));
            await DoPushDataAsync(input);
        }

        public async Task DoPushDataAsync(IMessage input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var message = input.Raw;
            if (stream.IsOpen)
            {
                log.Verbose($"{Name}: btr '{stream.BytesToRead}'");
                do
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                } while (stream.BytesToRead != 0);
                try
                {
                    await semaphore.WaitAsync();
                    if (IsRS485)
                    {
                        stream.RtsEnable = false;
                        //await Task.Delay(TimeSpan.FromMilliseconds(50));
                    }
                    stream.Write(message, 0, message.Length);
                    int k = 0;
                    do
                    {
                        if (k == 100)
                        {
                            break;
                        }
                        await Task.Delay(TimeSpan.FromMilliseconds(10));
                        k++;
                        continue;
                    } while (stream.BytesToWrite != 0);
                    if (IsRS485)
                    {
                        //await Task.Delay(TimeSpan.FromMilliseconds(50)); 
                        stream.RtsEnable = true;
                    }
                    var bytes = stream.BytesToWrite;
                    var size = stream.WriteBufferSize;
                    log.Verbose($"wrote to port {Name}: {message}, bytes {bytes}, buff_size {size}");
                }
                catch (Exception ex)
                {
                    log.Error($"Push data ex: {Name}, {ex.ToString()}");
                    if (IsRS485 && stream.RtsEnable != true)
                    {
                        stream.RtsEnable = true;
                        //await Task.Delay(TimeSpan.FromMilliseconds(50));
                    }
                    return;
                }
                finally
                {
                    if (IsRS485 && stream.RtsEnable != true)
                    {
                        stream.RtsEnable = true;
                    }
                    semaphore.Release();
                }
              
            }
            else
            {
                log.Error($"Cannot write to port: {Name}, port closed");
                return;
            }
            return;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            semaphore.Wait();
            stream.Close();
            semaphore.Dispose();
            isDisposed = true;
        }
    }
}
