﻿namespace Gibe.Umbraco.Blog.Models
{
	public interface IBlogSettings
	{
		string IndexName { get; }

		string BlogPostDocumentTypeAlias { get; }
	}
}
