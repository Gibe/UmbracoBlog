﻿using Umbraco.Core;
using Umbraco.Core.Composing;
using Gibe.Umbraco.Blog.Wrappers;
using Gibe.Umbraco.Blog.Models;
using Gibe.Settings.Interfaces;
using Gibe.Settings.Implementations;

namespace Gibe.Umbraco.Blog.Composing
{
	public class BlogComposer : IUserComposer
	{
		public void Compose(Composition composition)
		{
			composition.Components().Append<BlogDocumentTypeComponent>();
			composition.Components().Append<IndexEventsComponent>();

			composition.Register<ISettingsService, SettingsService>();

			composition.RegisterUnique<IBlogSettings, BlogConfigSettings>();
			composition.RegisterUnique<IBlogArchive, BlogArchive>();
			composition.RegisterUnique<IBlogAuthors, BlogAuthors>();
			composition.RegisterUnique<IBlogSearch, BlogSearch>();
			//composition.RegisterUnique<IBlogSections<BlogSectionsBase>, BlogSections<BlogSectionsBase>>();
			//composition.RegisterUnique<IBlogService<BlogPostBase>, BlogService<BlogPostBase>>();
			composition.RegisterUnique<IBlogTags, BlogTags>();
			composition.RegisterUnique<ISearchIndex, NewsIndex>();
		}
	}
}
