using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.Hosting;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Easy.Library.Lucene
{
    public interface ILuceneSeachModel
    {
        /// <summary>
        /// 数据主键
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 标题(进行检索)
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// 内容(进行检索)
        /// </summary>
        string Content { get; set; }
        /// <summary>
        /// 扩展参数1 (进行检索)
        /// </summary>
        string P1 { get; set; }
        /// <summary>
        /// 扩展参数2 (进行检索)
        /// </summary>
        string P2 { get; set; }
        /// <summary>
        /// 扩展参数3 (进行检索)
        /// </summary>
        string P3 { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        string LinkUrl { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        DateTime? AddTime { get; set; }
    }
    public static class LuceneHelper
    {
        private static string IndexPath
        {
            get
            {
                var luceneIndexPath = ConfigurationManager.AppSettings["LuceneIndexPath"];
                if (string.IsNullOrEmpty(luceneIndexPath))
                {
                    luceneIndexPath = HostingEnvironment.MapPath("/LuceneIndex");
                }
                return luceneIndexPath;
            }
        }

        public static Analyzer analyzer
        {
            //Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            get { return new PanGuAnalyzer(); }
        }
        public static void CreateIndex()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IndexWriter writer = null;
            
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(IndexPath), new NativeFSLockFactory());
            try
            {
                //该语句的作用：判断索引库文件夹是否存在以及索引特征文件是否存在。
                bool isExists = IndexReader.IndexExists(directory);
                if (isExists)
                {
                    //  如果索引目录被锁定（比如索引过程中程序异常退出），则首先解锁
                    //  Lucene.Net在写索引库之前会自动加锁，在close的时候会自动解锁
                    //  不能多线程执行，只能处理意外被永远锁定的情况
                    if (IndexWriter.IsLocked(directory))
                    {
                        //unlock:强制解锁，待优化
                        IndexWriter.Unlock(directory);  
                    }
                }

                writer = new IndexWriter(directory, analyzer, !isExists, IndexWriter.MaxFieldLength.UNLIMITED);
                //添加索引
                for (int i = 1; i <= 5; i++)
                {
                    Document document = new Document();
                    string path = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName + @"\Data\Test\" + i + ".txt";
                    string text = File.ReadAllText(path, Encoding.Default);
                    //Field.Store.YES:表示是否存储原值。只有当Field.Store.YES在后面才能用doc.Get("number")取出值来.Field.Index. NOT_ANALYZED:不进行分词保存
                    document.Add(new Field("number", i.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    // Lucene.Net.Documents.Field.TermVector.WITH_POSITIONS_OFFSETS:不仅保存分词还保存分词的距离。
                    document.Add(new Field("body", text, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                    writer.AddDocument(document);
                }
                writer.Optimize();
                sw.Stop();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                writer?.Dispose();
                directory?.Dispose();
            }
        }

        /// <summary>
        /// 对keyword进行分词，将分词的结果返回
        /// </summary>
        public static IEnumerable<string> SplitWords(string keyword)
        {
            IList<string> list = new List<string>();
            Analyzer analyzer = new PanGuAnalyzer();
            TokenStream stream = analyzer.TokenStream(keyword, new StringReader(keyword));
            ITermAttribute ita = null;
            bool hasNext = stream.IncrementToken();
            while (hasNext)
            {
                ita = stream.GetAttribute<ITermAttribute>();
                list.Add(ita.Term);
                hasNext = stream.IncrementToken();
            }
            return list;

            //IList<string> list = new List<string>();
            //Analyzer analyzer = new PanGuAnalyzer();
            //TokenStream tokenStream = analyzer.TokenStream("", new StringReader(keyword));
            //Token token = null;
            //while ((token = tokenStream.IncrementToken()) != null)
            //{
            //    // token.TermText()为当前分的词
            //    string word = token.TermText();
            //    list.Add(word);
            //}

            //return list;
        }
    }
}
