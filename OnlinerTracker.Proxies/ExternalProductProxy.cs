using Newtonsoft.Json;
using OnlinerTracker.Data;
using OnlinerTracker.Data.Context;
using OnlinerTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace OnlinerTracker.Proxies
{
    public class ExternalProductProxy : IExternalProductService
    {
        #region Properties and Fields

        protected string ServiceUrl;
        private readonly TrackerContext _context;

        #endregion

        #region Constructors

        public ExternalProductProxy(string srviceUrl)
        {
            this.ServiceUrl = srviceUrl;
            _context = new TrackerContext();
        }

        #endregion

        public List<ExternalProduct> Get(string searchQuery, Guid userId)
        {
            var url = string.Format("{0}?query={1}", ServiceUrl, searchQuery);
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.Accept = "application/json";
            request.Method = "GET";
            var responseString = string.Empty;
            var response = (HttpWebResponse) request.GetResponse();
            var encoding = Encoding.GetEncoding(response.CharacterSet);
            using (var responseStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(responseStream, encoding))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            var externalProductList = new  List<ExternalProduct>();
            dynamic jObjects = JsonConvert.DeserializeObject(responseString);
            foreach (var jObject in jObjects.products)
            {
                var externalProduct = new ExternalProduct
                {
                    OnlinerId = jObject["id"],
                    Name = jObject["full_name"],
                    Description = jObject["description"],
                    ImageUrl = jObject["images"]["header"],
                    CurrentCost = jObject["prices"] != null ? jObject["prices"]["min"] : 0,
                };
                var sameProduct = _context.Products.Any(u => u.OnlinerId == externalProduct.OnlinerId && u.UserId == userId);
                if (sameProduct)
                    externalProduct.Tracking = true;
                externalProductList.Add(externalProduct);
            }
            return externalProductList;
        }
    }
}
