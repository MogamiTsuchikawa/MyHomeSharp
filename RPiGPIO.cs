using System;
using System.IO;
using System.Threading;

namespace RaspberryPi
{
    public class GPIO
    {
        private const string BasePath = "/sys/class/gpio/";
        private bool isDebugMode = false;
        private const int GpioNum = 28;
        private bool?[] debugValue = new bool?[GpioNum];
        private Pin[] pins = new Pin[GpioNum];


        public GPIO(bool isDebugMode)
        {
            this.isDebugMode = isDebugMode;
        }
        protected string getValuePath(int number) => $"{BasePath}gpio{number}/value";
        protected string getDirectionPath(int number) => $"{BasePath}gpio{number}/direction";
        protected string getPinFolderPath(int number) => $"{BasePath}gpio{number}";
        protected string getUnExportPath() => $"{BasePath}unexport";
        protected string getExportPath() => $"{BasePath}export";

        public class Pin : IDisposable
        {
            public enum Kind
            {
                Input, Output
            }
            string GetKindText() => kind == Kind.Input ? "in" : "out";

            public Kind kind { get; private set; }
            public int number { get; private set; }
            private GPIO gpio { get; set; }
            public Pin(GPIO gpio, int number, Kind kind)
            {
                this.gpio = gpio;
                this.number = number;
                this.kind = kind;
                if (gpio.isDebugMode)
                {
                    Console.WriteLine($"gpio{number} is set {GetKindText()}put mode");
                    gpio.debugValue[number - 1] = false;
                }
                else
                {
                    if (!Directory.Exists(gpio.getPinFolderPath(number)))
                    {
                        File.WriteAllText(gpio.getExportPath(), number.ToString());
                    }
                    Thread.Sleep(10);
                    for(int i=0; !Directory.Exists(gpio.getPinFolderPath(number)) && i < 100; i++)
                    {
                        if(i==99){
                            Console.WriteLine("pin open timeout");
                            return;
                        }
                    }
                    File.WriteAllText(gpio.getDirectionPath(number), GetKindText());
                }
            }
            ~Pin()
            {
                if (gpio.isDebugMode)
                {
                    Console.WriteLine($"gpio{number} is clear");
                    gpio.debugValue[number - 1] = null;
                }
                else
                {
                    File.WriteAllText(gpio.getUnExportPath(), number.ToString());
                }
            }
            void IDisposable.Dispose()
            {
                if (gpio.isDebugMode)
                {
                    Console.WriteLine($"gpio{number} is clear");
                    gpio.debugValue[number - 1] = null;
                }
                else
                {
                    File.WriteAllText(gpio.getUnExportPath(), number.ToString());
                }
            }
            public bool value
            {
                get => gpio.isDebugMode ? gpio.debugValue[number - 1] == true : int.Parse(File.ReadAllText(gpio.getValuePath(number))) == 1;
                set
                {
                    if (gpio.isDebugMode)
                    {
                        gpio.debugValue[number - 1] = value;
                        Console.WriteLine($"gpio{number} is {value.ToString()}");
                    }
                    else File.WriteAllText(gpio.getValuePath(number), value ? "1" : "0");
                }
            }
        }
    }
}