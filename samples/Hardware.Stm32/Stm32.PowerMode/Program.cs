﻿using System;
using System.Threading;
using Windows.Devices.Gpio;
using STM32 = nanoFramework.Hardware.Stm32;

namespace Stm32.PowerMode
{
    public class Program
    {
        public static void Main()
        {
            /////////////////////////////////////////////////////////////
            // mind to set a pin that exists on the board being tested //
            /////////////////////////////////////////////////////////////

            // PJ5 is LD2 in STM32F769I_DISCO
            GpioPin led = GpioController.GetDefault().OpenStm32Pin('J', 5);
            // PD15 is LED6 in DISCOVERY4
            //GpioPin led = GpioController.GetDefault().OpenStm32Pin('D', 15);
            // PG14 is LEDLD4 in F429I_DISCO
            //GpioPin led = GpioController.GetDefault().OpenStm32Pin('G', 6);
            // PE15 is LED1 in QUAIL
            //GpioPin led = GpioController.GetDefault().OpenStm32Pin('E', 15);
            // PB75 is LED2 in STM32F746_NUCLEO
            //GpioPin led = GpioController.GetDefault().OpenStm32Pin('B', 7);
            // PA5 is LED_GREEN in STM32F091RC
            //GpioPin led = GpioController.GetDefault().OpenStm32Pin('A', 5);

            led.SetDriveMode(GpioPinDriveMode.Output);
            led.Write(GpioPinValue.High);

            // query target about wake-up reason
            switch (STM32.Power.WakeupReason)
            {
                case STM32.Power.WakeupReasonType.FromPin:
                    Console.WriteLine("[INFO] Device woke-up on GPIO event.");
                    break;

                case STM32.Power.WakeupReasonType.FromStandby:
                    Console.WriteLine("[INFO] Device woke-up from standby (possibly from alarm).");
                    break;

                case STM32.Power.WakeupReasonType.Undetermined:
                    Console.WriteLine("[INFO] Couldn't determine woke-up reason.");
                    break;
            }

            // enable wake-up from GPIO pin
            STM32.Power.EnableWakeupPin(STM32.Power.WakeupPin.Pin1);

            // start a thread blinking the LED to check that something is happening 
            new Thread( () => {
                while (true)
                {
                    Thread.Sleep(125);
                    led.Toggle();
                }
            }).Start();

            // set alarm time for 30 seconds from now
            DateTime alarmTime = DateTime.UtcNow.AddSeconds(30);

            STM32.RTC.SetAlarm(alarmTime);

            Console.WriteLine($"Setting alarm to {alarmTime.ToString("u")}");

            // read back alarm setting, just to be sure
            var alarmTimeCheck = STM32.RTC.GetAlarm();

            if( (alarmTimeCheck.Year != alarmTime.Year) ||
                (alarmTimeCheck.Month != alarmTime.Month) ||
                (alarmTimeCheck.Day != alarmTime.Day) ||
                (alarmTimeCheck.Hour != alarmTime.Hour) ||
                (alarmTimeCheck.Minute != alarmTime.Minute) ||
                (alarmTimeCheck.Second != alarmTime.Second)) 
            {
                Console.WriteLine($"!!!! ERROR: alarm time read from target is different from the set one: {alarmTimeCheck.ToString("u")}");
            }
            else
            {
                Console.WriteLine($"Alarm was set to {alarmTime.ToString("u")}");

                // sleep here for 10 seconds to allow the LED to blink after wakeup
                Thread.Sleep(10000);

                Console.WriteLine($"Going to standby mode now...");

                // this call never returns
                // after this the target will enter SMT32 CPU standby mode and will be waked by the RTC alarm in 30 - 10 seconds
                STM32.Power.EnterStandbyMode();
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
