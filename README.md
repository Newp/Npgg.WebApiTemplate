# Npgg.WebApiTemplate
 닷넷코어로 web api 를 만들때 자주 사용하는 유형들에 대해 정리



WIP : 작성중


## Middleware의 목적

하나의 request에 대한 흐름을 제어하기 위해서는 굳이 Middleware 없이도 동작이 가능하다.

하지만 그 기능이 많아질수록 하나의 함수에서 이를 제어하기가 복잡해지기 때문에 이를 Middleware 단위로 분리하여 각각 필요한 기능을 수행하도록 분리한다.

Middleware에는 실행 전 작업(pre processing), 실행 후 작업(post proccessing) 으로 나뉜다.

```csharp
public async Task InvokeAsync(HttpContext context, RequestDelegate next)
{
   // 여기에 pre process 
   await next(context);
   // 여기에 post process
}
```

실행 전 작업 : 실제로 사용자가 해당 api를 사용할 수 있는지, 또는 서버가 가능한 상태인지를 검사한다. 이 경우 api의 동작을 막을 수 있다.
ex1) 사용자가 정상적인 인증을 거친 이용자인지 검사
ex2) 서버가 현재 동작이 가능한 상태인지, 특정 기능이 점검중인지 선행검사

실행 후 작업 : api가 동작한 후에 마무리작업, 이 경우 api의 실행을 막을 수 없다.
ex1) api가 실행된 후에 request/response에 대한 log를 남긴다.
ex2) idempotent를 지킬수 있도록 실행 후 결과값을 별도로 저장한다.


## Middleware의 순서

pre/post 처리 여부에 따라서 선택한다.

다만 Stack처럼 A,B,C 세개의 middleware에서 InvokeAsync가 수행될 때, 아래처럼 수행된다.




A => B => C => (API ROUTE) => C => B => A

위 순서처럼 먼저넣은대로 넣은 순서의 역순으로 마무리가 된다. 

## 각 Middleware 의 순서 및 역할

ApiLoggingMiddleware : 최종적으로 유저에게 response를 마무리하기 전 request/response 관련된 값을 로그로 남긴다.
TryCatchMiddleware 
AuthenticationMiddleware
IdempotentMiddleware
AutholizationMiddleware
BufferMiddleware

 



### 예외처리, TryCatchMiddleware

 의도한/ 의도하지 않은 예외 모두 사용자에게는 '처리된 메세지'로서 전달되어야 한다.

예를 들어 db사용하는곳은 아주 많은곳이 있고, 모든곳에서 TryCatch 로 db error 를 핸들링 해주는것은 손이 많이가는 작업이다.

그렇다고 처리를 안해버리면 장애가 발생했을 때 사용자에게 서버의 문제점이 곧장 노출될 수 있다.

최악의 경우에는 db의 정보가 사용자에게 노출되는 사고로 이어질 수도 있다.

이때 API Action 보다 상위단계인 Middleware에서 아래와 같이 예외처리를 해준다면 몇가지 이득이 있다.

```csharp
public class TryCatchMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch(DatabasseException dbex)
        {
            context.Response.StatusCode = HttpStatusCode.ServiceUnavailable;
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}
```

- API Action 안에서 처리하는 예외처리는 개발자의 숙련도에 따라 예외처리에 실패할 수도 있다. 미들웨어에서 예외처리의 최소값을 둘 수 있다.

- 만약 json등으로 server-client 통신이 포멧이 약속된 상태라면 deserialize 를 실패시키지 않고, 정상적인 예외처리를 기대할 수 있다.

- 모든곳에서 처리해줘야 하는 DB에서 발생하는 예외처리를 상대적으로 간소화시킬 수 있다.

- throw new [HandledException]() = 지정한 '처리된 예외'를 발생시켜서 필요에 따라 동작하고 있는 코드의 위치를 곧장 Middleware 로 밀어낼 수 있다.


### 인증, Authentication 

 UseAuthentication를 사용하여 정해진 oauth2 를 구현하는것이 가능하지만
 
서비스 내부적으로 갖고 있는 token storage나 기타 인증방식, 

또는 UseAuthentication 없이 만드는것이 더 쉽다고 느껴지는 경우에 사용할 수 있다.
 
권한(Ahthorization)보다 먼저 실행해야 한다.


### 권한, Authrolization

 권한에는 짧게 보면 익명-사용자-관리자 정도로 나눌 수 있다.
 
하지만 실제 서비스를 작업하다 보면 계층을 하나의 수직구조로 볼 수 없다.

ex) 익명-사용자-관리자, 무료회원-유료구독회원, 일반유저-귀환이벤트유저

단순화 시킬 수 있는 반복잡업들에 대해 복붙을 하여

코드가 복잡해지고 이에 대한 관리마저 제대로 이루어지지 않아 버그를 만들어 내는 경우들이 있습니다.

이런 경우에는 Attribute 를 선언하고 이를 활용하여 권한 서비스에서 처리하도록 동작을 위임할 수 있습니다.

AuthrolizationMiddleware는 API Action 에 선언된 권한과 관련된 Attribute를 찾아 

API에 도달하기 전에 기능을 해결할 수 있도록 도움을 줄것입니다.

(작성중)
