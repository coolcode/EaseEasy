using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.ServiceModel.Syndication;

namespace CoolCode.Web.Mvc.Syndication {
	public static class FeedExtensions {
		public static FeedModel ToFeedModel(this SyndicationFeed feed) {
			Func<SyndicationItem, string> getHtmlContent = item => {
				if (item.Content != null) {
					return ((TextSyndicationContent)item.Content).Text;
				}
				return item.Summary == null ? null : item.Summary.Text; 
			};

			FeedModel model = new FeedModel {
				ID = feed.Id,
				Url = feed.Links.Any() ? feed.Links[0].Uri.AbsoluteUri : null,
				Email = feed.Authors.Any() ? feed.Authors[0].Email : null,
				UserName = feed.Authors.Any() ? feed.Authors[0].Name : null,
				Title = feed.Title == null ? null : feed.Title.Text,
				Text = feed.Copyright == null ? null : feed.Copyright.Text,
				Description = feed.Description == null ? null : feed.Description.Text
			};

			model.Categories = feed.Categories.Select(c => new FeedCategory {
				ID = c.Label,
				Title = c.Name,
				Description = c.Scheme
			}).ToList();

			model.Items = feed.Items.Select(c => new FeedItem {
				ID = c.Id,
				Title = c.Title == null ? null : c.Title.Text,
				Description = c.Summary == null ? null : c.Summary.Text,
				Email = c.Authors.Any() ? c.Authors[0].Email : null,
				UserName = c.Authors.Any() ? (c.Authors[0].Name ?? c.Authors[0].Email) : null,
				Text = getHtmlContent(c),
				Url = c.BaseUri != null ? c.BaseUri.AbsoluteUri : (c.Links.Any() ? c.Links[0].Uri.AbsoluteUri : c.Id),
				PublishDate = c.PublishDate.DateTime,
				LastUpdatedTime = (c.LastUpdatedTime < new DateTime(1900, 1, 1) ? c.PublishDate : c.LastUpdatedTime).DateTime
			}).ToList();

			return model;
		}

		public static SyndicationFeed ToSyndicationFeed(this FeedModel model) {
			SyndicationFeed syFeed = new SyndicationFeed(model.Title, model.Description, new Uri(model.Url));
			syFeed.Copyright = new TextSyndicationContent(model.Text);
			syFeed.Language = "zh-cn";
			syFeed.Authors.Add(new SyndicationPerson(model.Email, model.UserName, model.Url));

			foreach (var category in model.Categories) {
				SyndicationCategory sc = new SyndicationCategory();
				sc.Name = category.Title;
				sc.Scheme = category.Description;
				sc.Label = category.Title;
				syFeed.Categories.Add(sc);
			}

			List<SyndicationItem> items = new List<SyndicationItem>();
			foreach (var p in model.Items) {
				SyndicationItem item = new SyndicationItem();
				item.Authors.Add(new SyndicationPerson(p.Email, p.UserName, model.Url));
				item.BaseUri = new Uri(model.Url);
				item.Content = new TextSyndicationContent(p.Text);
				item.Categories.Add(new SyndicationCategory(p.Category.Title));
				item.AddPermalink(new Uri(p.Url));
				item.Links.Add(new SyndicationLink(new Uri(p.Url)));
				item.Id = p.ID;
				item.LastUpdatedTime = p.LastUpdatedTime;
				item.PublishDate = p.PublishDate;
				item.Title = new TextSyndicationContent(p.Title);
				items.Add(item);
			}
			syFeed.Items = items;

			return syFeed;
		}
	}
}
