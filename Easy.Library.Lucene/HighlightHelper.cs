using PanGu;
using PanGu.HighLight;

namespace Easy.Library.Lucene
{
    public static class HighlightHelper
    {
        /// <summary>
        /// 搜索结果高亮显示
        /// </summary>
        /// <param name="keyword"> 关键字 </param>
        /// <param name="content"> 搜索结果 </param>
        /// <returns> 高亮后结果 </returns>
        public static string HighLight(string keyword, string content)
        {
            // 创建HTMLFormatter,参数为高亮单词的前后缀
            var simpleHtmlFormatter = new SimpleHTMLFormatter("<span class=\"highlight\">", "</span>");
            // 创建 Highlighter ，输入HTMLFormatter 和 盘古分词对象Semgent
            var highlighter = new Highlighter(simpleHtmlFormatter, new Segment());
            //设置每个摘要段的字符数 
            highlighter.FragmentSize = 100;
            //获取最匹配的摘要段 
            return highlighter.GetBestFragment(keyword, content);
        }
    }
}
