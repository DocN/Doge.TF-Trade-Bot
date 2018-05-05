using SteamKit2;
using System.Collections.Generic;
using SteamTrade;
using System;
using System.IO;

namespace SteamBot
{
    public class SimpleUserHandler : UserHandler
    {
        //items to deliver
        public int[] skus;
        public int n = 0;
        public int ScrapPutUp;
        public int ordersCount = 0;
        public string[] orders;
        public string orderFile;
        public bool allFilled = true;
        public string directory;
        public string directoryCom;
        string orderNumber = "";
        string username = "";

        public void getOrder() {
        
        }



        public SimpleUserHandler (Bot bot, SteamID sid) : base(bot, sid) {}

        public override bool OnGroupAdd()
        {
            return false;
        }

        public override bool OnFriendAdd () 
        {
            return true;
        }

        public override void OnLoginCompleted()
        {
        }

        public override void OnChatRoomMessage(SteamID chatID, SteamID sender, string message)
        {
            Log.Info(Bot.SteamFriends.GetFriendPersonaName(sender) + ": " + message);
            base.OnChatRoomMessage(chatID, sender, message);
        }

        public override void OnFriendRemove () {}
        
        public override void OnMessage (string message, EChatEntryType type) 
        {
            Bot.SteamFriends.SendChatMessage(OtherSID, type, Bot.ChatResponse);
        }

        public override bool OnTradeRequest() 
        {
            return true;
        }
        
        public override void OnTradeError (string error) 
        {
            Bot.SteamFriends.SendChatMessage (OtherSID, 
                                              EChatEntryType.ChatMsg,
                                              "Oh, there was an error: " + error + "."
                                              );
            Bot.log.Warn (error);
        }
        
        public override void OnTradeTimeout () 
        {
            Bot.SteamFriends.SendChatMessage (OtherSID, EChatEntryType.ChatMsg,
                                              "Sorry, but you were AFK and the trade was canceled.");
            Bot.log.Info ("User was kicked because he was AFK.");
        }
        
        public override void OnTradeInit() 
        {
            allFilled = true;
            List<int> list = new List<int>();
            List<int> backOrder = new List<int>();

            if (IsAdmin)
            { 
                if (isDRN)
                {
                    Trade.SendMessage("Welcome DrN Owner of doge.TF Badass status");
                }
                else if (isUnit)
                {
                    Trade.SendMessage("Welcome to doge.TF Trade bot Mi-chan ^^");
                }
                else
                {
                    Trade.SendMessage("Welcome Admin!");
                }     
            }

            Trade.SendMessage("Welcome to doge.TF Trading Bot ~*SUCH KEYS, MUCH HATS, VERY WOW*~");
            n = 3;
            //read all files in directory
            directory = "orders/" + steam64ID.ToString() + "/";
            directoryCom = directory + "completed";
            try
            {
                //check if the directories needed exist
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
                if (!System.IO.Directory.Exists(directoryCom))
                {
                    System.IO.Directory.CreateDirectory(directoryCom);
                }

                orders = Directory.GetFiles(directory);
                ordersCount = orders.Length;
            }
            catch (Exception e)
            {
                
            }
            if (ordersCount <= 0)
            {
                Trade.SendMessage("Sorry, But you don't have any pending orders with Doge.TF");
                Trade.SendMessage("If you have a problem with an order please e-mail support@Doge.TF");
            }
            else
            {
                directory = orders[0];
                try
                {
                    string line;

                    // Read the file and display it line by line.
                    System.IO.StreamReader file =
                       new System.IO.StreamReader(directory);
                    if ((line = file.ReadLine()) != null)
                    {
                        username = line;
                    }
                    if ((line = file.ReadLine()) != null)
                    {
                        orderNumber = line;
                    }

                    Trade.SendMessage("Now filling Order #" + orderNumber + " for " + username);
                    
                    while ((line = file.ReadLine()) != null)
                    {
                        list.Add(Convert.ToInt32(line));
                        n++;
                    }
                    file.Close();
                }
                catch (Exception e)
                {
                    
                }
            }
            skus = list.ToArray();
            n = skus.Length;
            //count for loop
            int i = 0;
            //add all items on order list
            while (i < n)
            {
                if (Trade.AddItemByDefindex(skus[i]) == false)
                {
                    var schemaItem = Trade.CurrentSchema.GetItem(skus[i]);
                    Trade.SendMessage("Warning Item " + schemaItem.Name + " is out of stock!");
                    allFilled = false;
                    backOrder.Add(skus[i]);
                }
                i++;
            }
        }
        
        public override void OnTradeAddItem (Schema.Item schemaItem, Inventory.Item inventoryItem) {
            if (IsAdmin)
            {
                Trade.SendMessage(schemaItem.Defindex + "");
                Trade.SendMessage(inventoryItem.Id + "");
                Trade.SendMessage(inventoryItem.OriginalId + "");
                Trade.SendMessage(inventoryItem.Level + "");
                Trade.SendMessage(inventoryItem.Quality + "");
                Trade.SendMessage(inventoryItem.RemainingUses + "");
                Trade.SendMessage(inventoryItem.Origin + "");
                Trade.SendMessage(inventoryItem.Quality + "");

                Trade.SendMessage(inventoryItem.CustomName + "");
                Trade.SendMessage(inventoryItem.CustomDescription + "");

            }
        }
        
        public override void OnTradeRemoveItem (Schema.Item schemaItem, Inventory.Item inventoryItem) {}
        
        public override void OnTradeMessage (string message) {}
        
        public override void OnTradeReady (bool ready) 
        {
            Trade.SendMessage("ready!");
            if (!ready)
            {
                Trade.SetReady (false);
            }
            else
            {
                if(Validate ())
                {
                    Trade.SetReady (true);
                }
            }
        }

        public override void OnTradeSuccess()
        {
            // Trade completed successfully
            Log.Success("Trade Complete.");
        }

        public override void OnTradeAccept() 
        {
            if (Validate() || IsAdmin)
            {
                //Even if it is successful, AcceptTrade can fail on
                //trades with a lot of items so we use a try-catch
                try {
                    if (Trade.AcceptTrade())
                        Log.Success("Trade Accepted!");
                    if (allFilled == true)
                    {
                        Log.Success(allFilled.ToString());
                        Trade.SendMessage(directory + directoryCom + orderNumber);
                        System.IO.File.Move(directory, directoryCom + "/" + orderNumber);
                    }
                }
                catch {
                    Log.Warn ("The trade might have failed, but we can't be sure.");
                }
            }

            OnTradeClose ();
        }

        public bool Validate()
        {
            return true;
        }
    }
 
}

