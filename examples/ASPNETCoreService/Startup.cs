using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimCaptcha;

namespace AspNetCoreService
{
    public class Startup
    {
        readonly string VCodeAllowSpecificOrigins = "_VCodeAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ��Ҫ: ע����֤������, ֮��Ϳ����ڿ����� ͨ��������ע��
            services.Configure<SimCaptchaOptions>(Configuration.GetSection(
                                        SimCaptchaOptions.SimCaptcha));
            // ���� AspNetCoreClient ��������
            services.AddCors(options =>
            {
                options.AddPolicy(name: VCodeAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://example.com",
                                                          "https://localhost:44379")

                                      // �������json,������������: https://blog.csdn.net/yangyiboshigou/article/details/78738228
                                      // �������: Access-Control-Allow-Headers: Content-Type
                                      // TODO: ����,��Asp.Net Core��, Actionʵ���β�ֻ�ܴ�json, ������application/x-www-form-urlencoded, ���� HTTP 415, ԭ�����: Ӧ������Ϊ�ͻ��˷�������ʱ�õ�json��ʽ��û��ת��ΪFormData��ʽ
                                      // �ο�: https://www.cnblogs.com/jpfss/p/10102132.html
                                      .WithHeaders("Content-Type");
                                  });
            });

            // ���ڻ�ȡip��ַ
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // ���� SimCaptcha.AspNetCore.LocalCache ����
            services.AddMemoryCache();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // ����: ���� CORS �м��
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}