using OnlinerTracker.Core;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace OnlinerTracker.Proxies
{
    public class NbrbProxy : ICurrencyService
    {
        #region Properties and Fields

        private const string NbrbCurrencyApiUrl = "http://www.nbrb.by/Services/XmlExRates.aspx";

        #endregion

        #region IExternalProductService methods

        public Currency GetCurrent(CurrencyType currencyType)
        {
            if (currencyType == CurrencyType.BLR)
            {
                return new Currency {Type = CurrencyType.BLR, Value = 1};
            }
            var url = string.Format("{0}?ondate={1}/{2}/{3}", NbrbCurrencyApiUrl, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year);
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            string responseString;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                return null;
            }
            var encoding = Encoding.GetEncoding(response.CharacterSet);
            using (var responseStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(responseStream, encoding))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            dynamic externalCurrencies = DynamicXml.Parse(responseString);
            var currency = new Currency {Type = currencyType};
            foreach (var externalCurrency in externalCurrencies.Currency)
            {
                if (externalCurrency.CharCode as string == currencyType.ToString())
                {
                    currency.Value = Convert.ToDecimal(externalCurrency.Rate);
                }
            }
            return currency;
        }

        #endregion

    }
}
