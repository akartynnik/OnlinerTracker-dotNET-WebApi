using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace OnlinerTracker.Proxies
{
    public class ExternalProductProxy : IExternalProductService
    {
        #region Properties and Fields

        public string RemoteServiceType { get; set; }
        public string RemoteServiceUrl { get; set; }
        private readonly IProductService _productService;

        #endregion

        #region Constructors
        public ExternalProductProxy()
        {
        }

        public ExternalProductProxy(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        public List<Product> Get(string searchQuery, Guid userId, int page)
        {
            var url = string.Format("{0}?query={1}", RemoteServiceUrl, searchQuery);
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
            var poductList = _productService.ConvertToProducts(responseString,
                (RemoteServiceType) Convert.ToInt32(RemoteServiceType), userId);
            return poductList;
        }
    }
}
