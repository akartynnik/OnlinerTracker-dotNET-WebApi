using Newtonsoft.Json;
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

        public List<Product> Get(string searchQuery, Guid userId, int page)
        {
            var url = string.Format("{0}?query={1}", OnlinerApiUrl, searchQuery);
            if (page > 1)
                url = string.Format("{0}&page={1}", url, page);
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
            var poductList = ConvertToProducts(responseString, userId);
            return poductList;
        }

        #endregion

        #region Additional methods

        public List<Product> ConvertToProducts(string externalProductsJsonString, Guid userId)
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
                var ifSameProductExist = _productService.IfSameProductExist(product.OnlinerId, userId);
                if (ifSameProductExist)
                    product.Tracking = true;
                productList.Add(product);
            }
            return productList;
        }

        #endregion

    }
}
