using System;
using RaspberryPi;
using System.Threading;
class Program
{
    static void Main(string[] args)
    {
        var gpio = new GPIO(true);
        using GPIO.Pin greenLed = new(gpio, 21, GPIO.Pin.Kind.Output);
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        app.MapGet("/", () => "Hello World!");
        app.MapGet("/green", ()=>{
            greenLed.value = !greenLed.value;
            return greenLed.value.ToString();
        });
        app.Run();
        
        for(int i=0; i<300; i++)
        {
            greenLed.value = !greenLed.value;
            Thread.Sleep(500);
        }

    }
}