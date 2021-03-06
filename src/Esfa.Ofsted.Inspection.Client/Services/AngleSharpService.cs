﻿using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class AngleSharpService: IAngleSharpService
    {
        private readonly IHttpGet _httpGet;

        internal AngleSharpService() : this(new HttpService())
        {
        }

        internal AngleSharpService(IHttpGet httpGet)
        {
            _httpGet = httpGet;
        }

        public IList<string> GetLinks(string url, string selector, string textInTitle)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new List<string>();
            }

            var urlDetails = _httpGet.Get(url);
            var parser = new HtmlParser();
            var result = parser.Parse(urlDetails);
            var allMatchingItems = result.QuerySelectorAll(selector);

            return allMatchingItems
                .Where(x => x.InnerHtml.Trim().StartsWith(textInTitle, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.GetAttribute("href"))
                .ToList();
        }
    }
}
