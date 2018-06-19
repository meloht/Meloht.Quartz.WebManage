using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Meloht.Quartz.JobCore
{
    public class JobConfig
    {
        public const string MysqlConString = "Server=127.0.0.1; Database=quartznet_test; Uid=root; Pwd=lht123";
        public const string SchedulerName = "QuartzScheduler";//quartz.scheduler.instanceName

        public const string CallbackUrl = "CallbackUrl";
        public const string CallbackParams = "CallbackParams";

        public const string JobGroupNameHttp = "JobGroupNameHttp";
        public const string JobGroupNameAssembly = "JobGroupNameAssembly";

        public const string TriggerNameHttpPrefix = "TriggerNameHttp";
        public const string TriggerNameAssemblyPrefix = "TriggerNameAssembly";

        public const string TriggerGroupNameHttp = "TriggerGroupNameHttp";
        public const string TriggerGroupNameAssembly = "TriggerGroupNameAssembly";

        public const string QuartzXmlFileName = "quartz_jobs.xml";

        public const string JobGroupNameOnce = "JobGroupNameOnce";
        public const string TriggerGroupNameOnce = "TriggerGroupNameOnce";
        public const string TriggerNameOncePrefix = "TriggerNameOnce";

        public static string GetTriggerNameHttp(string jobName)
        {
            return $"{TriggerNameHttpPrefix}_{jobName}";
        }
        public static string GetTriggerNameAssembly(string jobName)
        {
            return $"{TriggerNameAssemblyPrefix}_{jobName}";
        }
        public static string GetTriggerNameOnce(string jobName)
        {
            return $"{TriggerNameOncePrefix}_{jobName}";
        }

        public static void SetJobStoreType(JobStoreType jobStoreType)
        {
            _jobStoreType = jobStoreType;
        }
        public static JobStoreType JobStoreType { get { return _jobStoreType; } }

        private static JobStoreType _jobStoreType;


        public static NameValueCollection GetJobConfig()
        {
            if (JobStoreType == JobStoreType.AdoJobStore)
                return GetJobAdoStoreCfg();
            return GetJobXmlCfg();
        }

        public static NameValueCollection GetJobXmlCfg()
        {
            var properties = new NameValueCollection();

            //存储类型
            properties["quartz.scheduler.instanceName"] = SchedulerName;


            properties["quartz.scheduler.instanceId"] = "AUTO";
            properties["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz";

            properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins";
            properties["quartz.plugin.xml.fileNames"] = "quartz_jobs.xml";

            return properties;
        }

        public static NameValueCollection GetJobAdoStoreCfg()
        {
            var properties = new NameValueCollection();

            //存储类型
            properties["quartz.scheduler.instanceName"] = SchedulerName;
            properties["quartz.scheduler.instanceId"] = "AUTO";

            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            properties["quartz.serializer.type"] = "json";
            //表名前缀
            properties["quartz.jobStore.tablePrefix"] = "qrtz_";
            //驱动类型
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz";
            //数据源名称
            properties["quartz.jobStore.dataSource"] = "myDS";
            //连接字符串
            properties["quartz.dataSource.myDS.connectionString"] = MysqlConString;
            //版本
            properties["quartz.dataSource.myDS.provider"] = "MySql";


            return properties;
        }
    }
}
