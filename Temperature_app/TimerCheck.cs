using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Mail;
using System.Net;
using Microsoft.Win32.TaskScheduler;


namespace TemperatureApp
{
    public class TimerCheck
    {
        public static void RunTimer(SpreadsheetFile sf)
        {
            string receiver = "";
            
            Console.Write("Please enter the email address you'd like the data to be sent to: ");
            bool validated = false;
            while (!validated)
            {
                receiver = Console.ReadLine();
                if (InputValidation.IsValidEmailAddress(receiver))
                {
                    validated = true;
                }
                else
                {
                    Console.WriteLine("Sorry, this email address is invalid, please try again: ");
                }
            }
            
            Console.WriteLine("How often would you like to be emailed (in hours)?");

            bool numValid = false;
            float interval = 0f;
            while (!numValid) {
                if (float.TryParse(Console.ReadLine(), out interval)) {
                    numValid = true;
                }
                else
                {
                    Console.Write("Number of hours was invalid, please try again: ");
                }
            }

            //get service on local machine
            using (TaskService ts = new TaskService())
            {
                //set up new task definition
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Send emails concerning thermometer data";

                //create trigger 
                DailyTrigger dt = new DailyTrigger();
                dt.DaysInterval = 1;
                dt.Repetition.Interval = TimeSpan.FromHours(interval);
                td.Triggers.Add(dt);

                //assign action of sending an email evertime the tigger is activated
                td.Actions.Add(@"C:\Users\peter.macaldowie\Documents\Visual Studio 2015\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\ConsoleApplication1.exe", $"{receiver} {sf.pathname}");

                // Register the task in the root folder of the local machine
                TaskService.Instance.RootFolder.RegisterTaskDefinition("Send Temperature Data", td);
            }
            
        }
    }
}
