using Newtonsoft.Json;
using OnlinerTracker.Core;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace OnlinerTracker.Proxies
{
    public class OnlinerProxy : IExternalProductService
    {
        #region Properties and Fields

        private const string OnlinerApiUrl = "https://catalog.api.onliner.by/search/products";
        private readonly IProductService _productService;
        public static string OnlinerApiUrl1 => OnlinerApiUrl;

        #endregion

        #region Constructors

        public OnlinerProxy(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        #region IExternalProductService methods

        public string Get(string searchQuery, int page)
        {
            var url = string.Format("{0}?query={1}", OnlinerApiUrl, searchQuery.Replace(" ", "+"));
            if (page > 1)
                url = string.Format("{0}&page={1}", url, page);
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = "application/json";
            request.MediaType = "application/json";
            request.Accept = "application/json";
            request.Method = "GET";
            var responseString = string.Empty;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                return string.Empty;
            }
            var encoding = Encoding.GetEncoding(response.CharacterSet);
            using (var responseStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(responseStream, encoding))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            return responseString;
        }

        public List<Product> ConvertJsonToProducts(string externalProductsJsonString, Guid? userId = null)
        {
            var productList = new List<Product>();
            dynamic externalProducts = JsonConvert.DeserializeObject(externalProductsJsonString);
            foreach (var externalProduct in externalProducts["products"])
            {
                var product = new Product
                {
                    OnlinerId = externalProduct["id"],
                    Name = externalProduct["full_name"],
                    Description = externalProduct["description"],
                    ImageUrl = externalProduct["images"]["header"],
                    CurrentCost = externalProduct["prices"] != null ? externalProduct["prices"]["min"] : 0
                };
                if (userId != null)
                {
                    var ifSameProductExist = _productService.IfSameProductExist(product.OnlinerId, userId.ToGuid());
                    if (ifSameProductExist)
                        product.Tracking = true;
                }
                productList.Add(product);
            }
            return productList;
        }

        #endregion

    }
}
