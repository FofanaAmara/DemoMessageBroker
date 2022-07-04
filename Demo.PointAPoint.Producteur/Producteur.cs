using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.PointAPoint.Producteur
{
    public class Producteur
    {

        //private string _chaineConnexion = "Endpoint=sb://demobrokercgi.servicebus.windows.net/;SharedAccessKeyName=root;SharedAccessKey=L3VvL8UuaQadc9RWVt1jUQjEPW33vrZt+ZUEalz5AKI=;EntityPath=queue1";
        private string _chaineConnexion = "Endpoint=sb://testamara.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1gtLsYmXRXW9hhcxoQgbmgLf1a4eOPh15OsGQFYDlnU=";
        private string _nomFile = "queue1";


        public List<ServiceBusMessage> GenererMessages(int nombreMessages)
        {
            var messages = new List<ServiceBusMessage>();

            for (int i = 1; i <= nombreMessages; i++)
            {
                messages.Add(new ServiceBusMessage("Message " + i));
            }

            return messages;
        }

        public async Task ProduireAsync(List<ServiceBusMessage> messages)
        {
            var client = new ServiceBusClient(_chaineConnexion);

            ServiceBusSender producteur = client.CreateSender(_nomFile);

            var chrono = new Stopwatch();

            chrono.Start();

            foreach (var message in messages)
            {
                Console.WriteLine("Envoi de \"" + message.Body.ToString() + "\" vers le broker ...");

                try
                {
                    await producteur.SendMessageAsync(message);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Message envoyé avec suucès !");
                    Console.ForegroundColor = ConsoleColor.White;

                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Une erreur est survenue lors de l'envoi : " + e.Message);
                    Console.ForegroundColor = ConsoleColor.White;

                }
            }

            chrono.Stop();

            ReporterDureeTransfertMessages(chrono, messages.Count);

            TraitementMessageHeure(chrono, messages.Count);
        }

        private static void ReporterDureeTransfertMessages(Stopwatch chrono, int nbMessages)
        {
            var time = TimeSpan.FromMilliseconds(chrono.ElapsedMilliseconds);

            Console.WriteLine("Durée transfert de {0} messages : {1} minutes {2} secondes {3} millisecondes",
                nbMessages.ToString(),
                time.Minutes.ToString(),
                time.Seconds.ToString(),
                time.Milliseconds.ToString());
        }

        private static void TraitementMessageHeure(Stopwatch chrono, int nbMessage)
        {
            var time = (float)(chrono.ElapsedMilliseconds);

            var messageHeure = ((float)nbMessage / time) * 3600000;
            var messageHeureArrondi = Math.Round(messageHeure, 0);

            Console.WriteLine("Débit : {0} messsages/heure", messageHeureArrondi.ToString());
        }
    }
}
