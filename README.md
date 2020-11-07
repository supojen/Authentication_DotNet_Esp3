# Authorization 
<br><br>

### .Net Core Authorization 機制
___
* 大方向概念
    - Building AuthorizationHanlder to handle the AuthorizationRequirement
    - Using AuthorizationPolicyBuilder to build AuthorizationPolicy with AuthorizationRequirements
* 3個基本名詞
    1. AuthorizationPolicy
        - 就像是政策一樣,一個政策是由襖多個執法人員(Handler),來執行法規(Requirement)而形成的
    1. AuthorizationHandler
        - 就像是執行法規的人,像是法官一樣,法官來判斷被授權者是否符合法規(Requirement)
    1. AuthorizationRequirement
        - 就像是法規一樣,被授權者必遵守這些 Requirement

* 建立 AuthorizationRequirement & AuthorizationHandler 的邏輯
    - AuthorizationRequirement 會帶入一些 AuthorizationHandler 需要用到的參數.意思是,只會 Keep 一些參數不會做任何邏輯判斷
    - AuthorizationHandler 會利用 AuthorizationRequirement 帶入的參數來做一些邏輯判斷,主要是判斷是否擁有權限.當然,他也可以自己從比如說 DbContext 要其他參數進來, 但速度可能會慢些,因為多了一些 I/O 的事.

* 建立 AuthorizationPolicy 的邏輯
    - 注意, 一定會透過 AuthorizationPolicyBuilder 來建立
    - 在建立 Policy 時, 抽象概念是, 我要符合什麼 AuthorizationRequirement, 我就要把什麼 AutjorizationRequirement 加入 Policy 內
    - 最後, 必須 DI AuthorizationHandler 進來. 
    - 我的 Policy 必須符合啥 requirements( AuthorizationPolicyBuilder build policy with reqirements ), 最後我必須雇用什麼 handlers 來完成這些 requirements (Dependency Inject Handler)




<br><br><br>
## 範例
<br><br>

### 1. 改寫 Deafult Authorize Policy
___
* Register Authorization middleware with the below setting 

    1. 建立一個 AuthorizationPolicyBuiler
        ```c# 
        var defaultPolicyBuiler = new AuthorizationPolicyBuilder(); 
        ```
    1. 利用剛建立的 AuthorizationPolicyBuilder 建立 Policy
        ```c#
        var defaultPolicy = defaultPolicyBuiler
                            .RequireAuthenticatedUser() // 必須走身分才能 Authorize
                            .RequireClaim("level")      // 必須有 claim "level" 才能 Authorize
                            .Build();
        ```
    1. 用剛剛建立的 Policy 覆蓋掉 Deafult 的 Policy
        ```c#
        options.DefaultPolicy = defaultPolicy;
        ```
* 整體代碼如下
    ```c#
    services.AddAuthorization(options => 
    {
        var defaultPolicyBuiler = new AuthorizationPolicyBuilder(); 
        var defaultPolicy = defaultPolicyBuiler.RequireAuthenticatedUser()
                                        .RequireClaim("god")
                                        .Build();

        options.DefaultPolicy = defaultPolicy;
    });
    ```

<br><br>
### 2. 自定義 Authorization Policy
___
*  方式一

    1. 創建 **AuthorizationRequirement**
        ```c#
        public class CustomRequireClaim : IAuthorizationRequirement
        {
            public string ClaimType { get; set; }
            public CustomRequireClaim(string claimType)
            {
                ClaimType = claimType;
            }
        }
        ```
    1. 創建 **AuthorizationHandler**
        ```c#
        public class CustomRequireClaimHanlder : AuthorizationHandler<CustomRequireClaim>
        {
            protected override Task HandleRequirementAsync(
                AuthorizationHandlerContext context, 
                CustomRequireClaim requirement)
            {
                var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
                if(hasClaim)
                {
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }
        }
        ```
    1. Register the new **AuthorizationPolicy** with the custom AuthorizarionRequirement
        ```c#
        services.AddAuthorization( options => {

            options.AddPolicy("Claim.Level", policyBuilder =>
            {
                policyBuilder.AddRequirements(new CustomRequireClaim("level"));
            });
        });
        ```
    1. **DI the AuthorizationHandler**
        ```c#
        services.AddScoped<IAuthorizationHandler, CustomRequireClaimHanlder>();
        ```
* 方式二 (與方式一不一樣的地方是 Register AuthorizationPolicy 的部分)

    1. 建立一個 AuthorizationPolicyBuilder 的 Extentions
        ```c#
        public static class AuthorizationPolicyBuilderExtensions
        {
            public static AuthorizationPolicyBuilder RequireCustomClaim(
                this AuthorizationPolicyBuilder builder,
                string claim)
            {
                builder.AddRequirements(new CustomRequireClaim(claim));
                return builder;
            }
        }
        ```
    1. 用 extension build new AuthorizationPolicy
        ```c#
        services.AddAuthorization( options => {
            options.AddPolicy("Claim.Level", policyBuilder => 
            {
                policyBuilder.RequireCustomClaim("level");
            });
        });
        ```