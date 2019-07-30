﻿using Examine;
using Examine.Providers;
using System;
using System.Globalization;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Core.Events;
using Gibe.Umbraco.Blog.Exceptions;
using Gibe.Umbraco.Blog.Extensions;

namespace Gibe.Umbraco.Blog.Composing
{
	public class IndexEvents : IComponent
	{
		private readonly IExamineManager _examineManager;
		private readonly IUserService _userService;

		private const string IndexName = "ExternalIndex";

		public IndexEvents(IExamineManager examineManager,
			IUserService userService)
		{
			_examineManager = examineManager;
			_userService = userService;
		}

		public void Initialize()
		{
			_examineManager.TryGetIndex(IndexName, out var index);

			if (index == null)
			{
				throw new IndexNotFoundException(IndexName);
			}

			ContentService.Saving += ContentServiceSaving;
			((BaseIndexProvider)index).TransformingIndexValues += ExternalIndexTransformingIndexValues;
		}

		private void ExternalIndexTransformingIndexValues(object sender, IndexingItemEventArgs e)
		{
			var document = e.ValueSet;

			if (document.GetSingleValue<string>("nodeTypeAlias") != "blogPost")
			{
				return;
			}

			var postDate = DateTime.ParseExact(document.GetSingleValue<string>("postDate").Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
			document.TryAdd("postDateYear", postDate.Year.ToString("0000"));
			document.TryAdd("postDateMonth", postDate.Month.ToString("00"));
			document.TryAdd("postDateDay", postDate.Day.ToString("00"));

			var authorId = Convert.ToInt32(document.GetSingleValue<string>("postAuthor"));
			document.TryAdd("postAuthorName", GetUserName(authorId).ToLower());

			var tags = document.GetSingleValue<string>("settingsNewsTags");
			if (tags != null)
			{
				foreach (var tag in tags.Split(','))
				{
					document.TryAdd("tag", tag.ToLower());
				}
			}

			var path = document.GetSingleValue<string>("path");
			if (path != null)
			{
				foreach (var id in path.Split(','))
				{
					document.TryAdd("path", id);
				}
			}
		}

		private void ContentServiceSaving(IContentService sender, ContentSavingEventArgs e)
		{
			foreach (var entity in e.SavedEntities)
			{
				try
				{
					if (entity.ContentType.Alias != "BlogPost" || entity.ParentId == -20)
					{
						continue;
					}

					// TODO : Move code to somewhere better
					var parentContent = sender.GetById(entity.ParentId);
					if (parentContent.Published)
					{
						//if the date hasn't been set, default it to today
						var postDate = DateTime.Now.Date;
						var postDateString = entity.GetValue<string>("postDate");
						if (string.IsNullOrEmpty(postDateString))
						{
							entity.SetValue("postDate", postDate);
						}
					}
				}
				catch (InvalidOperationException)
				{
					// This happens if you try to get ParentId during install of a package with content
				}
			}
		}

		private string GetUserName(int userId)
		{
			return _userService.GetUserById(userId).Name;
		}

		public void Terminate()
		{
			
		}
	}
}
