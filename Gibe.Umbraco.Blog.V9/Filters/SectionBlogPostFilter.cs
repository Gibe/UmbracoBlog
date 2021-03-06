﻿using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class SectionBlogPostFilter : IBlogPostFilter
	{
		private readonly int _sectionNodeId;

		public SectionBlogPostFilter(int sectionNodeId)
		{
			_sectionNodeId = sectionNodeId;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.Field(ExamineFields.Path, _sectionNodeId.ToString());
		}
	}
}