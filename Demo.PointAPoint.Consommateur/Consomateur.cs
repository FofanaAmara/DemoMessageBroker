using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.PointAPoint.Consommateur
{
    public class Consomateur
    {
        //private string _chaineConnexion = "Endpoint=sb://demobrokercgi.servicebus.windows.net/;SharedAccessKeyName=root;SharedAccessKey=L3VvL8UuaQadc9RWVt1jUQjEPW33vrZt+ZUEalz5AKI=;EntityPath=queue1";
        private string _chaineConnexion = "Endpoint=sb://testamara.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=1gtLsYmXRXW9hhcxoQgbmgLf1a4eOPh15OsGQFYDlnU=";

        private string _nomFile = "queue1";

        public async void RecevoirAsync(int quantiteARecevoir)
        {
            var client = new ServiceBusClient(_chaineConnexion);

            ServiceBusReceiver consommateur = client.CreateReceiver(_nomFile);

            var compteur = 0;
            var chrono = new Stopwatch();

            chrono.Start();

            while (true)
            {
                Console.WriteLine("En attente de message sur la file : " + _nomFile + " ...");

                try
                {
                    ServiceBusReceivedMessage messageRecu = await consommateur.ReceiveMessageAsync(TimeSpan.FromMilliseconds(2000));

                    if (messageRecu is null)
                        continue;

                    string contenuMessage = messageRecu.Body.ToString();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Message recu : " + contenuMessage);
                    Console.ForegroundColor = ConsoleColor.White;

                    await consommateur.CompleteMessageAsync(messageRecu);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Confirmation de traitement Envoyée  : " + contenuMessage);
                    Console.ForegroundColor = ConsoleColor.White;

                    compteur++;

                    if(compteur == quantiteARecevoir)
                    {
                        chrono.Stop();

                        ReporterDureeTransfertMessages(chrono, quantiteARecevoir);

                        TraitementMessageHeure(chrono, quantiteARecevoir);
                    }


                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Une erreur est survenue lors de la reception du message : " + e.Message);
                    Console.ForegroundColor = ConsoleColor.White;

                }

            }

            
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
