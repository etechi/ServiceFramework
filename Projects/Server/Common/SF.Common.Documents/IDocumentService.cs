#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0


using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.Threading.Tasks;

namespace SF.Common.Documents
{
	public class ListArgument : PagingArgument
	{
		/// <summary>
		/// 目录ID，若不指定返回根目录中的对象
		/// </summary>
		public long? Category { get; set; }

		/// <summary>
		/// 文档区域
		/// </summary>
		public string Scope { get; set; }
	}

	public class SearchArgument : PagingArgument
	{
		/// <summary>
		/// 搜索关键字
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// 文档区域
		/// </summary>
		public string Scope { get; set; }
	}
	/// <summary>
	/// 文档服务
	/// </summary>
	/// <typeparam name="TDocument"></typeparam>
	/// <typeparam name="TCategory"></typeparam>
	[NetworkService]
	public interface IDocumentService<TDocument, TCategory>
		where TDocument : Document
		where TCategory : Category
	{
		/// <summary>
		/// 通过ID获取文档
		/// </summary>
		/// <param name="Id">主键</param>
		/// <returns>文档</returns>
		Task<TDocument> GetDocument(long Id);

		/// <summary>
		/// 通过快速访问键值获取文档
		/// </summary>
		/// <param name="Id">快速访问键值</param>
		/// <param name="Scope">文档区域</param>
		/// <returns>文档</returns>
		Task<TDocument> GetDocumentByKey(string Id, string Scope = null);

		/// <summary>
		/// 通过ID获取文档目录
		/// </summary>
		/// <param name="Id">目录ID</param>
		/// <returns>目录</returns>
		Task<TCategory> GetCategory(long Id);

		/// <summary>
		/// 通过关键字搜索文档
		/// </summary>
		/// <param name="Arg">搜索参数</param>
		/// <returns>结果集</returns>
		Task<QueryResult<TDocument>> Search(SearchArgument Arg);

		/// <summary>
		/// 获取目录中的文档列表
		/// </summary>
		/// <param name="Arg">文档列表参数</param>
		/// <returns>文档列表</returns>
		Task<QueryResult<TDocument>> ListDocuments(ListArgument Arg);

		/// <summary>
		/// 获取目录中的子目录列表
		/// </summary>
		/// <param name="Arg">子目录参数</param>
		/// <returns>子目录列表</returns>
		Task<QueryResult<TCategory>> ListCategories(ListArgument Arg);

	}

	public interface IDocumentService : IDocumentService<Document, Category>
	{

	}
}
