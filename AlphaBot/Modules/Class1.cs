using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace firstBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {


        [Command("stockx")]
        public async Task getStockX([Remainder] string stylecodeInp)
        {
            if (stylecodeInp == "help")
            {
                var embed = new EmbedBuilder();
                embed.WithAuthor("StockX Price Finder");
                embed.WithTitle("Help");
                embed.AddField("All sizes:", "!stockx <itemname> \nThis will output the highest bid and lowest ask for all of the sizes.", false);
                embed.AddField("One size:", "!stockx <itemname>, <US shoe size> \nHighest bid, lowest ask and total payout for the given size.", false);
                embed.AddField("Clothing:", "!stockx <itemname> \nThis will output the highest bid and lowest ask for all of the sizes.", false);
                embed.WithCurrentTimestamp();
                embed.WithFooter("Shark Bots");
                embed.WithColor(Color.Green);

                await ReplyAsync(embed: embed.Build());
            }
            else
            {
                string[] inputArray = stylecodeInp.Split(", ");
                string stylecode = inputArray[0];

                //Display all sizes
                if (inputArray.Length == 1)
                {
                    try
                    {

                        string urlAddress = "https://stockx.com/api/browse?productCategory=sneakers&currency=GBP&_search=" + stylecode + "&dataType=product";

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                        request.Method = "GET";
                        request.UserAgent = "Mozilla/5.0 (X11; CrOS x86_64 10066.0.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {

                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;

                            if (String.IsNullOrWhiteSpace(response.CharacterSet))
                            {
                                readStream = new StreamReader(receiveStream);
                            }

                            else
                            {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }

                            string data = readStream.ReadToEnd();



                            int pFrom = data.IndexOf("title\":\"") + "title\":\"".Length;
                            int pTo = data.IndexOf("\",\"traits");

                            string productName = data.Substring(pFrom, pTo - pFrom);

                            int pFrom1 = data.IndexOf("\"deadstockRangeLow\":") + "\"deadstockRangeLow\":".Length;
                            int pTo1 = data.IndexOf(",\"deadstockRangeHigh\"");

                            string deadstockRangeLow = data.Substring(pFrom1, pTo1 - pFrom1);

                            int pFrom2 = data.IndexOf("\"deadstockRangeHigh\":") + "\"deadstockRangeHigh\":".Length;
                            int pTo2 = data.IndexOf(",\"volatility\"");

                            string deadstockRangeHigh = data.Substring(pFrom2, pTo2 - pFrom2);

                            string deadstockRange = "£" + deadstockRangeLow + " - " + "£" + deadstockRangeHigh;

                            int pFrom3 = data.IndexOf("\"smallImageUrl\":\"") + "\"smallImageUrl\":\"".Length;
                            int pTo3 = data.IndexOf("\",\"thumbUrl\"");

                            string productImg = data.Substring(pFrom3, pTo3 - pFrom3);

                            int pFrom4 = data.IndexOf("salesThisPeriod\":") + "salesThisPeriod\":".Length;
                            int pTo4 = data.IndexOf(",\"salesLastPeriod");

                            string salesLast72Hours = data.Substring(pFrom4, pTo4 - pFrom4);

                            int pFrom5 = data.IndexOf("shortDescription\":\"") + "shortDescription\":\"".Length;
                            int pTo5 = data.IndexOf("\",\"styleId");

                            string urlName = data.Substring(pFrom5, pTo5 - pFrom5);

                            int pFrom6 = data.IndexOf(",\"lowestAsk\":") + ",\"lowestAsk\":".Length;
                            int pTo6 = data.IndexOf(",\"lowestAskSize");

                            string lowestAskPrice = data.Substring(pFrom6, pTo6 - pFrom6);

                            int pFrom7 = data.IndexOf("\"lowestAskSize\":\"") + "\"lowestAskSize\":\"".Length;
                            int pTo7 = data.IndexOf("\",\"parentLowestAsk");

                            string lowestAskSize = data.Substring(pFrom7, pTo7 - pFrom7);

                            int pFrom8 = data.IndexOf(",\"highestBid\":") + ",\"highestBid\":".Length;
                            int pTo8 = data.IndexOf(",\"highestBidSize\"");

                            string highestBidPrice = data.Substring(pFrom8, pTo8 - pFrom8);

                            int pFrom9 = data.IndexOf("\"highestBidSize\":\"") + "\"highestBidSize\":\"".Length;
                            int pTo9 = data.IndexOf("\",\"numberOfBids\"");

                            string highestBidSize = data.Substring(pFrom9, pTo9 - pFrom9);

                            int pFrom10 = data.IndexOf("\"id\":\"") + "\"id\":\"".Length;
                            int pTo10 = data.IndexOf("\",\"uuid");

                            string productID = data.Substring(pFrom10, pTo10 - pFrom10);


                            double highestBidSizeInt = Convert.ToDouble(highestBidSize);
                            double payoutPrice = Convert.ToDouble(highestBidPrice);
                            payoutPrice = payoutPrice - (((payoutPrice / 100) * 9.5) + ((payoutPrice / 100) * 3.5) + 8.5);
                            string sizeGuide;
                            if (highestBidSizeInt < 7)
                            {
                                sizeGuide = "**Most profitable sizes:** 4 - 7";
                            }
                            else if (highestBidSizeInt > 7 && highestBidSizeInt < 10)
                            {
                                sizeGuide = "**Most profitable sizes:** 7 - 10";
                            }
                            else
                            {
                                sizeGuide = "**Most Profitable sizes:** 10 - 14";
                            }

                            // get specific size info

                            string urlProductName = productName.Replace(" ", "-");
                            string urlAddress2 = "https://stockx.com/" + urlName;

                            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(urlAddress2);
                            request2.Method = "GET";
                            request2.UserAgent = "Mozilla/5.0 (X11; CrOS x86_64 10066.0.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                            HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();

                            if (response2.StatusCode == HttpStatusCode.OK)
                            {
                                Stream receiveStream2 = response2.GetResponseStream();
                                StreamReader readStream2 = null;

                                if (String.IsNullOrWhiteSpace(response2.CharacterSet))
                                    readStream2 = new StreamReader(receiveStream2);
                                else
                                    readStream2 = new StreamReader(receiveStream2, Encoding.GetEncoding(response2.CharacterSet));


                                string data2 = readStream2.ReadToEnd();

                                int fromD2 = data2.IndexOf("\"lowestAskSize\"") + "\"lowestAskSize\"".Length;
                                int toD2 = data2.LastIndexOf("shoeSize");
                                string newData2 = data2.Substring(fromD2, toD2 - fromD2);

                                int fromD21 = data2.IndexOf("retailPrice\":");
                                string retailPre = data2.Substring(fromD21, (fromD21 + 20) - fromD21);
                                string retailPost = "";
                                for (int i = 0; i < retailPre.Length; i++)
                                {
                                    if (char.IsNumber(retailPre[i]))
                                    {
                                        retailPost += retailPre[i];
                                    }
                                }


                                string[] sizeArray = { "4", "4.5", "5", "5.5", "6", "6.5", "7", "7.5", "8", "8.5", "9", "9.5", "10", "10.5", "11", "11.5", "12", "12.5", "13", "13.5", "14" };
                                string[] highestBidArray = { "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", };
                                string[] lowestAskArray = { "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", };
                                for (int i = 0; i < highestBidArray.Length; i++)
                                {
                                    try
                                    {
                                        highestBidArray[i] = getHighestBid(newData2, sizeArray[i]);
                                        lowestAskArray[i] = getLowestAsk(data2, sizeArray[i]);
                                    }

                                    catch
                                    {

                                    }

                                }

                                var embed = new EmbedBuilder();
                                embed.WithAuthor("StockX Price Finder");
                                embed.WithTitle(productName)
                                    .WithUrl("https://stockx.com/" + urlName);
                                embed.WithDescription(">>> **Retail:** $" + retailPost + "\n**Popularity:** " + salesLast72Hours + " sales in the last 72 hours\n" + sizeGuide + "\n**Most profitable size:** US " + highestBidSize + " with total payout of £" + payoutPrice.ToString());
                                embed.AddField("\u200B", "**Lowest Ask | Highest Bid**", false);
                                for (int i = 0; i < sizeArray.Length; i++)
                                {
                                    embed.AddField("US " + sizeArray[i], "\n£" + lowestAskArray[i] + " | £" + highestBidArray[i], true);
                                }
                                embed.WithThumbnailUrl(productImg);
                                embed.WithCurrentTimestamp();
                                embed.WithFooter("Shark Bots");
                                embed.WithColor(Color.Green);

                                await ReplyAsync(embed: embed.Build());

                            }





                        }
                        else
                        {
                            await ReplyAsync("Test unsuccessful");
                        }
                    }
                    catch
                    {
                        string urlAddress = "https://stockx.com/api/browse?productCategory=streetwear&_search=" + stylecode;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                        request.Method = "GET";
                        request.UserAgent = "Mozilla/5.0 (X11; CrOS x86_64 10066.0.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {

                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;

                            if (String.IsNullOrWhiteSpace(response.CharacterSet))
                                readStream = new StreamReader(receiveStream);
                            else
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));


                            string data = readStream.ReadToEnd();


                            int pFrom = data.IndexOf("title\":\"") + "title\":\"".Length;
                            int pTo = data.IndexOf("\",\"traits");

                            string productName = data.Substring(pFrom, pTo - pFrom);

                            int pFrom1 = data.IndexOf("\"deadstockRangeLow\":") + "\"deadstockRangeLow\":".Length;
                            int pTo1 = data.IndexOf(",\"deadstockRangeHigh\"");

                            string deadstockRangeLow = data.Substring(pFrom1, pTo1 - pFrom1);

                            int pFrom2 = data.IndexOf("\"deadstockRangeHigh\":") + "\"deadstockRangeHigh\":".Length;
                            int pTo2 = data.IndexOf(",\"volatility\"");

                            string deadstockRangeHigh = data.Substring(pFrom2, pTo2 - pFrom2);

                            string deadstockRange = "£" + deadstockRangeLow + " - " + "£" + deadstockRangeHigh;

                            int pFrom3 = data.IndexOf("\"smallImageUrl\":\"") + "\"smallImageUrl\":\"".Length;
                            int pTo3 = data.IndexOf("\",\"thumbUrl\"");

                            string productImg = data.Substring(pFrom3, pTo3 - pFrom3);

                            int pFrom4 = data.IndexOf("salesThisPeriod\":") + "salesThisPeriod\":".Length;
                            int pTo4 = data.IndexOf(",\"salesLastPeriod");

                            string salesLast72Hours = data.Substring(pFrom4, pTo4 - pFrom4);

                            int pFrom5 = data.IndexOf("shortDescription\":\"") + "shortDescription\":\"".Length;
                            int pTo5 = data.IndexOf("\",\"styleId");

                            string urlName = data.Substring(pFrom5, pTo5 - pFrom5);

                            int pFrom6 = data.IndexOf(",\"lowestAsk\":") + ",\"lowestAsk\":".Length;
                            int pTo6 = data.IndexOf(",\"lowestAskSize");

                            string lowestAskPrice = data.Substring(pFrom6, pTo6 - pFrom6);

                            int pFrom7 = data.IndexOf("\"lowestAskSize\":\"") + "\"lowestAskSize\":\"".Length;
                            int pTo7 = data.IndexOf("\",\"parentLowestAsk");

                            string lowestAskSize = data.Substring(pFrom7, pTo7 - pFrom7);

                            int pFrom8 = data.IndexOf(",\"highestBid\":") + ",\"highestBid\":".Length;
                            int pTo8 = data.IndexOf(",\"highestBidSize\"");

                            string highestBidPrice = data.Substring(pFrom8, pTo8 - pFrom8);

                            int pFrom9 = data.IndexOf("\"highestBidSize\":\"") + "\"highestBidSize\":\"".Length;
                            int pTo9 = data.IndexOf("\",\"numberOfBids\"");

                            string highestBidSize = data.Substring(pFrom9, pTo9 - pFrom9);

                            double payoutPrice = Convert.ToDouble(highestBidPrice);
                            payoutPrice = payoutPrice - (((payoutPrice / 100) * 9.5) + ((payoutPrice / 100) * 3.5) + 8.5);
                            string sizeGuide;
                            if (highestBidSize == "S")
                            {
                                sizeGuide = "**Most profitable size:** Small";
                            }
                            else if (highestBidSize == "M")
                            {
                                sizeGuide = "**Most profitable size:** Medium";
                            }
                            else if (highestBidSize == "L")
                            {
                                sizeGuide = "**Most profitable size:** Large";
                            }
                            else
                            {
                                sizeGuide = "**Most Profitable size:** XLarge";
                            }

                            //specific size
                            string urlAddress2 = "https://stockx.com/" + urlName;

                            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(urlAddress2);
                            request2.Method = "GET";
                            request2.UserAgent = "Mozilla/5.0 (X11; CrOS x86_64 10066.0.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                            HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();

                            if (response2.StatusCode == HttpStatusCode.OK)
                            {
                                Stream receiveStream2 = response2.GetResponseStream();
                                StreamReader readStream2 = null;

                                if (String.IsNullOrWhiteSpace(response2.CharacterSet))
                                    readStream2 = new StreamReader(receiveStream2);
                                else
                                    readStream2 = new StreamReader(receiveStream2, Encoding.GetEncoding(response2.CharacterSet));


                                string data2 = readStream2.ReadToEnd();

                                int fromD2 = data2.IndexOf("\"lowestAskSize\"") + "\"lowestAskSize\"".Length;
                                int toD2 = data2.LastIndexOf("shoeSize");
                                string newData2 = data2.Substring(fromD2, toD2 - fromD2);



                                int fromD21 = data2.IndexOf("\"Retail\",\"value\":");
                                string retailPre = data2.Substring(fromD21 + 15, (fromD21 + 25) - fromD21 + 15);
                                string retailPost = "";
                                for (int i = 0; i < retailPre.Length; i++)
                                {
                                    if (char.IsNumber(retailPre[i]))
                                    {
                                        retailPost += retailPre[i];
                                    }
                                }


                                string[] sizeArray = { "S", "M", "L", "XL" };
                                string[] highestBidArray = { "N/A", "N/A", "N/A", "N/A" };
                                string[] lowestAskArray = { "N/A", "N/A", "N/A", "N/A" };
                                for (int i = 0; i < highestBidArray.Length; i++)
                                {
                                    try
                                    {
                                        highestBidArray[i] = getHighestBid(newData2, sizeArray[i]);
                                        lowestAskArray[i] = getLowestAsk(data2, sizeArray[i]);
                                    }

                                    catch
                                    {

                                    }

                                }


                                var embed = new EmbedBuilder();
                                embed.WithAuthor("StockX Price Finder");
                                embed.WithTitle(productName)
                                    .WithUrl("https://stockx.com/" + urlName);
                                embed.WithDescription(">>> **Retail:** $" + retailPost + "\n**Popularity:** " + salesLast72Hours + " sales in the last 72 hours\n" + sizeGuide + " with total payout of £" + payoutPrice.ToString());
                                embed.AddField("Small:", "Highest Bid: £" + highestBidArray[0] + "\t Lowest Ask: £" + lowestAskArray[0], false);
                                embed.AddField("Medium:", "Highest Bid: £" + highestBidArray[1] + "\t Lowest Ask: £" + lowestAskArray[1], false);
                                embed.AddField("Large:", "Highest Bid: £" + highestBidArray[2] + "\t Lowest Ask: £" + lowestAskArray[2], false);
                                embed.AddField("XLarge:", "Highest Bid: £" + highestBidArray[3] + "\t Lowest Ask: £" + lowestAskArray[3], false);
                                embed.WithThumbnailUrl(productImg);
                                embed.WithCurrentTimestamp();
                                embed.WithFooter("Shark Bots");
                                embed.WithColor(Color.Green);

                                await ReplyAsync(embed: embed.Build());
                            }

                        }

                    }
                    
                }

                
                //Display single size
                else if (char.IsNumber(inputArray[1][1]))
                {


                    string urlAddress = "https://stockx.com/api/browse?productCategory=sneakers&currency=GBP&_search=" + stylecode + "&dataType=product";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                    request.Method = "GET";
                    request.UserAgent = "Mozilla/5.0 (X11; CrOS x86_64 10066.0.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        Stream receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;

                        if (String.IsNullOrWhiteSpace(response.CharacterSet))
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));


                        string data = readStream.ReadToEnd();



                        int pFrom = data.IndexOf("title\":\"") + "title\":\"".Length;
                        int pTo = data.IndexOf("\",\"traits");

                        string productName = data.Substring(pFrom, pTo - pFrom);

                        int pFrom1 = data.IndexOf("\"deadstockRangeLow\":") + "\"deadstockRangeLow\":".Length;
                        int pTo1 = data.IndexOf(",\"deadstockRangeHigh\"");

                        string deadstockRangeLow = data.Substring(pFrom1, pTo1 - pFrom1);

                        int pFrom2 = data.IndexOf("\"deadstockRangeHigh\":") + "\"deadstockRangeHigh\":".Length;
                        int pTo2 = data.IndexOf(",\"volatility\"");

                        string deadstockRangeHigh = data.Substring(pFrom2, pTo2 - pFrom2);

                        string deadstockRange = "£" + deadstockRangeLow + " - " + "£" + deadstockRangeHigh;

                        int pFrom3 = data.IndexOf("\"smallImageUrl\":\"") + "\"smallImageUrl\":\"".Length;
                        int pTo3 = data.IndexOf("\",\"thumbUrl\"");

                        string productImg = data.Substring(pFrom3, pTo3 - pFrom3);

                        int pFrom4 = data.IndexOf("salesThisPeriod\":") + "salesThisPeriod\":".Length;
                        int pTo4 = data.IndexOf(",\"salesLastPeriod");

                        string salesLast72Hours = data.Substring(pFrom4, pTo4 - pFrom4);

                        int pFrom5 = data.IndexOf("shortDescription\":\"") + "shortDescription\":\"".Length;
                        int pTo5 = data.IndexOf("\",\"styleId");

                        string urlName = data.Substring(pFrom5, pTo5 - pFrom5);

                        int pFrom6 = data.IndexOf(",\"lowestAsk\":") + ",\"lowestAsk\":".Length;
                        int pTo6 = data.IndexOf(",\"lowestAskSize");

                        string lowestAskPrice = data.Substring(pFrom6, pTo6 - pFrom6);

                        int pFrom7 = data.IndexOf("\"lowestAskSize\":\"") + "\"lowestAskSize\":\"".Length;
                        int pTo7 = data.IndexOf("\",\"parentLowestAsk");

                        string lowestAskSize = data.Substring(pFrom7, pTo7 - pFrom7);

                        int pFrom8 = data.IndexOf(",\"highestBid\":") + ",\"highestBid\":".Length;
                        int pTo8 = data.IndexOf(",\"highestBidSize\"");

                        string highestBidPrice = data.Substring(pFrom8, pTo8 - pFrom8);

                        int pFrom9 = data.IndexOf("\"highestBidSize\":\"") + "\"highestBidSize\":\"".Length;
                        int pTo9 = data.IndexOf("\",\"numberOfBids\"");

                        string highestBidSize = data.Substring(pFrom9, pTo9 - pFrom9);

                        int pFrom10 = data.IndexOf("\"id\":\"") + "\"id\":\"".Length;
                        int pTo10 = data.IndexOf("\",\"uuid");

                        string productID = data.Substring(pFrom10, pTo10 - pFrom10);


                        // get specific size info

                        string urlProductName = productName.Replace(" ", "-");
                        string urlAddress2 = "https://stockx.com/" + urlName;

                        HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(urlAddress2);
                        request2.Method = "GET";
                        request2.UserAgent = "Mozilla/5.0 (X11; CrOS x86_64 10066.0.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                        HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();

                        if (response2.StatusCode == HttpStatusCode.OK)
                        {
                            Stream receiveStream2 = response2.GetResponseStream();
                            StreamReader readStream2 = null;

                            if (String.IsNullOrWhiteSpace(response2.CharacterSet))
                                readStream2 = new StreamReader(receiveStream2);
                            else
                                readStream2 = new StreamReader(receiveStream2, Encoding.GetEncoding(response2.CharacterSet));


                            string data2 = readStream2.ReadToEnd();



                            double desiredSize = Convert.ToDouble(inputArray[1]);
                            double sizeMinusOneInt = desiredSize - 1;
                            string sizeMinusOne = sizeMinusOneInt.ToString();


                            string lowestAskDesiredSize = " ";
                            string highestBidDesiredSize = " ";


                            lowestAskDesiredSize = getLowestAsk(data2, inputArray[1].ToString());


                            highestBidDesiredSize = getHighestBid(data2, inputArray[1].ToString());

                            int fromD21 = data2.IndexOf("retailPrice\":");
                            string retailPre = data2.Substring(fromD21, (fromD21 + 20) - fromD21);
                            string retailPost = "";
                            for (int i = 0; i < retailPre.Length; i++)
                            {
                                if (char.IsNumber(retailPre[i]))
                                {
                                    retailPost += retailPre[i];
                                }
                            }

                            double highestBidSizeInt = Convert.ToDouble(highestBidSize);
                            string sizeGuide;
                            if (highestBidSizeInt < 7)
                            {
                                sizeGuide = "**Most profitable sizes:** 4 - 7";
                            }
                            else if (highestBidSizeInt > 7 && highestBidSizeInt < 10)
                            {
                                sizeGuide = "**Most profitable sizes:** 7 - 10";
                            }
                            else
                            {
                                sizeGuide = "**Most Profitable sizes:** 10 - 14";
                            }
                            double payoutPrice = Convert.ToDouble(highestBidDesiredSize);
                            payoutPrice = payoutPrice - (((payoutPrice / 100) * 9.5) + ((payoutPrice / 100) * 3.5) + 8.5);

                            double payoutPrice1 = Convert.ToDouble(highestBidPrice);
                            payoutPrice1 = payoutPrice1 - (((payoutPrice / 100) * 9.5) + ((payoutPrice / 100) * 3.5) + 8.5);

                            var embed = new EmbedBuilder();
                            embed.WithAuthor("StockX Price Finder");
                            embed.WithTitle(productName + " US" + desiredSize)
                                .WithUrl("https://stockx.com/" + urlName);
                            embed.WithDescription(">>> **Retail:** $" + retailPost + "\n**Popularity:** " + salesLast72Hours + " sales in the last 72 hours\n" + sizeGuide + "\n**Most profitable size:** US " + highestBidSize + " with total payout of £" + payoutPrice.ToString());
                            embed.AddField("Lowest ask: ", "£" + lowestAskDesiredSize, true);
                            embed.AddField("Highest bid: ", "£" + highestBidDesiredSize, true);
                            embed.AddField("Payout: ", "£" + payoutPrice.ToString(), true);
                            embed.WithThumbnailUrl(productImg);
                            embed.WithCurrentTimestamp();
                            embed.WithFooter("Shark Bots");
                            embed.WithColor(Color.Green);

                            await ReplyAsync(embed: embed.Build());
                        }


                    }
                    else
                    {
                        await ReplyAsync("Test unsuccessful");
                    }

                }
            }

        }


        public string getLowestAsk(string data, string size)
        {
            string lowestAskDesiredSize = " ";
            try
            {

                string newString = "";
                //double sizeMinusOneInt = size - 1;
                //string sizeMinusOne = sizeMinusOneInt.ToString();

                int foundIndex = data.IndexOf("lowestAskSize\":" + size.ToString() + ",\"");
                int newIndexStart = foundIndex - 8;
                int newIndexEnd = foundIndex - 2;
                string foundPrice = data.Substring(newIndexStart, newIndexEnd - newIndexStart);
                for (int i = 0; i < foundPrice.Length; i++)
                {
                    if (Char.IsNumber(foundPrice[i]))
                    {
                        newString = newString + foundPrice[i];
                    }
                }

                return newString;
            }
            catch
            {
                string newString = "";
                int foundIndex = data.IndexOf("lowestAskSize\":\"" + size.ToString() + "\",\"");
                int newIndexStart = foundIndex - 8;
                int newIndexEnd = foundIndex - 2;
                string foundPrice = data.Substring(newIndexStart, newIndexEnd - newIndexStart);
                for (int i = 0; i < foundPrice.Length; i++)
                {
                    if (Char.IsNumber(foundPrice[i]))
                    {
                        newString = newString + foundPrice[i];
                    }

                }
                if (newString == "")
                {
                    return "N/A";
                }
                return newString;
            }

            return lowestAskDesiredSize;

        }

        public string getHighestBid(string data, string size)
        {
            string highestBidDesiredSize = " ";
            try
            {

                string newString = "";
                //double sizeMinusOneInt = size - 1;
                //string sizeMinusOne = sizeMinusOneInt.ToString();

                int foundIndex = data.IndexOf("highestBidSize\":" + size.ToString() + ",\"");
                int newIndexStart = foundIndex - 8;
                int newIndexEnd = foundIndex - 2;
                string foundPrice = data.Substring(newIndexStart, newIndexEnd - newIndexStart);
                for (int i = 0; i < foundPrice.Length; i++)
                {
                    if (Char.IsNumber(foundPrice[i]))
                    {
                        newString = newString + foundPrice[i];
                    }
                }

                return newString;
            }
            catch
            {
                string newString = "";
                int foundIndex = data.IndexOf("highestBidSize\":\"" + size.ToString() + "\",\"");
                int newIndexStart = foundIndex - 8;
                int newIndexEnd = foundIndex - 2;
                string foundPrice = data.Substring(newIndexStart, newIndexEnd - newIndexStart);
                for (int i = 0; i < foundPrice.Length; i++)
                {
                    if (Char.IsNumber(foundPrice[i]))
                    {
                        newString = newString + foundPrice[i];
                    }

                }
                if (newString == "")
                {
                    return "N/A";
                }
                return newString;
            }

        }






    }
}