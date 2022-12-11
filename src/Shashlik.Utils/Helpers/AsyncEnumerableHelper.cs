using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.github.xiangyuecn.rsacsharp;

namespace Shashlik.Utils.Helpers;

/// <summary>
/// 异步迭代器
/// </summary>
public static class AsyncEnumerableHelper
{
    /// <summary>
    /// 反射方式执行异步迭代, await foreach 
    /// </summary>
    /// <param name="obj">异步迭代器对象</param>
    /// <param name="foreachAction">foreach执行器</param>
    /// <exception cref="InvalidCastException">错误的对象类型</exception>
    public static async void ForEach(object obj, Action<object?, Type> foreachAction)
    {
        var interfaces = obj.GetType().GetInterfaces();
        var asyncEnumerableInterface =
            interfaces.FirstOrDefault(r => r.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>));
        if (asyncEnumerableInterface is null)
            throw new InvalidCastException();

        var itemType = asyncEnumerableInterface.GenericTypeArguments.First();
        var methodInfo = asyncEnumerableInterface.GetMethod(nameof(IAsyncEnumerable<int>.GetAsyncEnumerator));
        var enumeratorObj = methodInfo!.Invoke(obj, null);
        var asyncEnumeratorType = typeof(IAsyncEnumerator<>).MakeGenericType(itemType);
        var moveNextMethod = asyncEnumeratorType.GetMethod(nameof(IAsyncEnumerator<int>.MoveNextAsync));
        var currentPro = asyncEnumeratorType.GetProperty(nameof(IAsyncEnumerator<int>.Current));

        try
        {
            while (true)
            {
                var valueTask = (moveNextMethod!.Invoke(enumeratorObj, null) as ValueTask<bool>?);
                if (!await valueTask!.Value)
                    break;
                var value = currentPro!.GetValue(enumeratorObj);
                foreachAction.Invoke(value, itemType);
            }
        }
        finally
        {
            await (enumeratorObj as IAsyncDisposable)!.DisposeAsync();
        }
    }
}