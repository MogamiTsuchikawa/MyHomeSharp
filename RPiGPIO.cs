using System;
using System.IO;

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
        protected string getUnExportPath() => $"{BasePath}unexport";

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
                get => gpio.isDebugMode ? gpio.debugValue[number - 1] == true : File.ReadAllText(gpio.getValuePath(number)) == "1";
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