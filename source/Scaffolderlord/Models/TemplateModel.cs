﻿using Scaffolderlord.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Scaffolderlord.Extensions;

namespace Scaffolderlord.Models
{
    public abstract class BaseTemplateModel
    {
        /// <summary>
        /// Returns an newline
        /// </summary>
        public string NewLine => "\r\n";

        /// <summary>
        /// Returns an tab for each level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public string Indent(int level = 1) => new string('\t', level);

        /// <summary>
        /// Creates simple XML doc
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public string XmlDoc(string comment)
        {
            return $@"
/// <summary>
/// {comment}
/// </summary>";
        }
    }

    public interface ITemplateModel
    {
        string TemplateFileName { get; }
        string? Namespace { get; }
        IEnumerable<string> Usings { get; }

        string GetTemplateFilePath() => Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates"), TemplateFileName);
        string GetOutputPath();
    }

}
