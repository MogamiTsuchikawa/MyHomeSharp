using System;
using RaspberryPi;
using System.Threading;
class Program
{
    static Timer stopBtnTimer;
    static void Main(string[] args)
    {
        var gpio = new GPIO(true);
        using GPIO.Pin greenLed = new(gpio, 21, GPIO.Pin.Kind.Output);
        using GPIO.Pin stopBtn = new(gpio, 20, GPIO.Pin.Kind.Input);
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        app.MapGet("/", () => "Hello World!");
        TimerCallback stopBtnCheck = state =>
        {
            if(!stopBtn.value){
                greenLed.value = false;
                stopBtnTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        };
        stopBtnTimer = new Timer(stopBtnCheck, null, Timeout.Infinite, Timeout.Infinite);

        app.MapGet("/green", ()=>{
            greenLed.value = !greenLed.value;
            return greenLed.value.ToString();
        });
        app.MapGet("/come", ()=>{
            greenLed.value = true;
            stopBtnTimer.Change(3, 3);
            return "ok";
        });
        app.Run();
        

    }
}