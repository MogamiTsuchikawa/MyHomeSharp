using System;

namespace RaspberryPi
{
    public class GPIO
    {
        private const string basePath = "/sys/class/gpio/";
        private bool isDebugMode = false;

        public GPIO(bool isDebugMode)
        {
            this.isDebugMode = isDebugMode;
        }

        public void Write(Pin pin, bool value)
        {
            pin.Write();
        }

        public class Pin
        {
            public enum Kind
            {
                Input = "in", Output = "out"
            }

            public Kind kind { get; private set; }
            public int number { get; private set; }
            public Pin(int number, Kind kind)
            {
                this.number = number;
                this.kind = kind;
            }
            protected void Write(bool value)
            {

            } 
        }
    }
}