using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemoteFetch.Classes
{
    class FetchExecutor
    {
        public async Task ExecuteTask(FetchUnit fetchUnit, DateTime dateTimeNow)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");

            HttpResponseMessage response = await httpClient.GetAsync(fetchUnit.Url);

            string pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);

            foreach (FetchItem fetchItem in fetchUnit.FetchItems)
            {
                HtmlNode xpathValue = pageDocument.DocumentNode.SelectSingleNode(fetchItem.Xpath);
                bool isSuccessfull = false;
                string fetchedValue = "";

                if (xpathValue != null)
                {
                    if (xpathValue.InnerText == null || String.IsNullOrEmpty(xpathValue.InnerText))
                    {
                        foreach (HtmlAttribute attribute in xpathValue.Attributes)
                        {
                            if (attribute.Value == null || String.IsNullOrEmpty(attribute.Value))
                            {
                                Console.WriteLine($"The script is having trouble with processing the provided {fetchItem.Xpath}.");
                            }
                            else
                            {
                                //We can asume that parsing was somewhat sucessfull
                                isSuccessfull = true;

                                if (fetchItem.FetchValueParser == null)
                                {
                                    fetchedValue = SanitizeString(attribute.Value);
                                }
                                else
                                {
                                    foreach (FetchValueParser fetchValueParser in fetchItem.FetchValueParser)
                                    {
                                        fetchedValue = SanitizeString(fetchValueParser.ParseCustom(fetchValueParser, attribute.Value));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //We can asume that parsing was somewhat sucessfull
                        isSuccessfull = true;
                        fetchedValue = SanitizeString(xpathValue.InnerText);
                    }

                    if(isSuccessfull)
                    {
                        FetchItemValue fetchItemValue = new FetchItemValue { ValueDateTime = dateTimeNow, Value = fetchedValue };
                        Console.WriteLine($"Value of {fetchItem.ItemName} is {fetchItemValue.Value}.");

                        if (fetchItem.FetchItemValues == null)
                        {
                            fetchItem.FetchItemValues = new List<FetchItemValue>();
                        }

                        fetchItem.FetchItemValues.Add(fetchItemValue);
                    }

                }
                else
                {
                    Console.WriteLine($"The item {fetchItem.ItemName} xpath {fetchItem.Xpath} yielded no results.");
                }
                
                
                if (fetchItem.FetchItemValues == null)
                {
                    fetchItem.FetchItemValues = new List<FetchItemValue>();
                }


            }
        }

        public string SanitizeString(string inputString)
        {
            inputString = Regex.Replace(inputString, @"[^\w\.@-]", "");
            inputString = inputString.Trim();

            return inputString;
        }
    }
}
