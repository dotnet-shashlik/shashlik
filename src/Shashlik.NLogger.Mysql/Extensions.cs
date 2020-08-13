using Guc.EfCore;
using Guc.Kernel;
using Guc.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using System.IO;
using System.Text;

namespace Guc.NLogger
{
    public static class NLoggerMysqlExtensions
    {
        /// <summary>
        /// 增加nlogger服务,需要在Program中调用UseNLog,使用postgres数据库
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="connString">日志数据库连接字符串</param>
        /// <param name="nLogXmlConfigContent">nlog 配置文件内容,空则使用内置配置</param>
        /// <param name="loggingConfigs">日志推送配置</param>
        /// <param name="autoMigration">是否启用自动迁移</param>
        /// <returns></returns>
        public static IKernelBuilder AddNLogWithMysql(
            this IKernelBuilder kernelBuilder,
            IConfigurationSection loggingConfigs,
            string nLogXmlConfigContent = null,
            bool autoMigration = false)
        {
            var services = kernelBuilder.Services;
            services.Configure<LoggingOptions>(loggingConfigs);
            var loggingOptions = loggingConfigs.Get<LoggingOptions>();

            #region 日志数据库

            kernelBuilder.Services.AddDbContext<LogDbContext>(dbOptions =>
            {
                dbOptions.UseMySql(loggingOptions.Conn, builder =>
                {
                    builder.MigrationsAssembly(typeof(LogDbContext).Assembly.GetName().FullName);
                });
            });

            if (autoMigration)
                kernelBuilder.Services.Migration<LogDbContext>();

            #endregion

            #region nlog 配置文件自动处理

            // 读取默认配置
            if (nLogXmlConfigContent.IsNullOrWhiteSpace())
            {
                using var stream = typeof(LogDbContext).Assembly.GetManifestResourceStream($"Guc.NLogger.Mysql.nlog.mysql.config");
                using var sm = new StreamReader(stream);
                nLogXmlConfigContent = sm.ReadToEnd();
            }

            string filename = $"./nlog.config";
            if (!File.Exists("./internal-nlog.txt"))
                File.Create("./internal-nlog.txt");

            StringBuilder ignores = new StringBuilder();
            if (!loggingOptions.Ignores.IsNullOrEmpty())
            {
                foreach (var item in loggingOptions.Ignores)
                {
                    ignores.AppendLine($"\t\t\t\t<when condition=\"{item}\" action=\"Ignore\" />");
                }
            }

            // 格式化配置文件
            nLogXmlConfigContent = nLogXmlConfigContent.RazorFormat(new
            {
                loggingOptions.Email,
                loggingOptions.Conn,
                Ignores = ignores.ToString()
            });
            File.WriteAllText(filename, nLogXmlConfigContent, Encoding.UTF8);

            NLogBuilder.ConfigureNLog(filename);

            #endregion


            return kernelBuilder;
        }
    }
}
