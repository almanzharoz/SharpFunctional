# SharpFunctional - библиотека для написания кода на C# в функциональном стиле.

Реальный пример:
Определение количества записей в ElasticSearch (драйвер Nest).

Пример кода без использования библиотеки:

		public int Count<T>(IQueryBuilder<T> query) where T : class, IEntity
		{
			var result = _client.Count<T>(x =>
			{
				var q = x.Type(_mapping.GetMappingType<T>().Discriminator);
				if (query != null)
					q = q.Query(y => ((ElasticQueryBuilder<T>) query).GetQueryContainer());
				return q;
			});
			if (!result.IsValid)
				throw new Exception($"Error in Count<{typeof(T)}>: {result.ServerError?.Error}");
			return (int)result.Count;
		}

Тот же функционал, но с использованием библиотеки:

		public int Count<T>(IQueryBuilder<T, ElasticEntityKey> query) where T : class, IEntity<ElasticEntityKey>
			=> _client.Count<T>(x => x.Type(_mapping.GetMappingType<T>().Discriminator)
				.If(q => query != null, q => q.Query(y => ((ElasticQueryBuilder<T>) query).GetQueryContainer()), q=>null))
				.ThrowIf(x => !x.IsValid, x => new Exception($"Error in Count<{typeof (T)}>: {x.ServerError?.Error}, {x.DebugInformation}"))
				.Convert(x => (int) x.Count);

Асинхронная версия:

		public Task<int> CountAsync<T>(IQueryBuilder<T, ElasticEntityKey> query) where T : class, IEntity<ElasticEntityKey>
			=> _client.CountAsync<T>(x => x.Type(_mapping.GetMappingType<T>().Discriminator)
				.If(q => query != null, q => q.Query(y => ((ElasticQueryBuilder<T>) query).GetQueryContainer()), q=>null))
				.ThrowIf(x => !x.IsValid, x => new Exception($"Error in Count<{typeof (T)}>: {x.ServerError?.Error}, {x.DebugInformation}"))
				.Convert(x => (int) x.Count);
				
Можно увидеть абсолютную схожесть кода в синхронной и асинхронной версиях. Это очень удобная особенность написания асинхронных функций, мы не используем в коде async/await.

# Еще пример
Action MVC-контроллера: Удаление элемента из БД, 1 функция для всех типов объектов

	[Route("{a}/{b}/{c}/{d}/delete")]
	[Route("{a}/{b}/{c}/delete")]
	[Route("{a}/{b}/delete")]
	[Route("{a}/delete")]
	[HttpGet]
	public IActionResult Delete(string a, string b, string c, string d) =>
		_dal.GetUrl(a, b, c, d)
		.SwitchNotNull(_dal.GetPage, x => (IActionResult)PartialView(x))
		.SwitchNotNull(_dal.GetCategoryByUrl, PartialView)
		.SwitchNotNull(_dal.GetNewsByUrl, PartialView)
		.SwitchNotNull(_dal.GetActionByUrl, PartialView)
		.SwitchNotNull(_dal.GetProduct, PartialView)
		.SwitchNotNull(_dal.GetProductCategoryByUrl, PartialView)
		.SwitchNotNull(_dal.GetProducer, PartialView)
		.SwitchDefault(NotFound);

	[Route("{a}/{b}/{c}/{d}/delete")]
	[Route("{a}/{b}/{c}/delete")]
	[Route("{a}/{b}/delete")]
	[Route("{a}/delete")]
	[HttpPost]
	public IActionResult Delete(string a, string b, string c, string d, string url, bool noredirect=false) =>
		_dal.GetUrl(a, b, c, d)
		.SwitchNotNull(_dal.GetPage, x => noredirect.If(x, url, _dal.Delete, _dal.Delete))
		.SwitchNotNull(_dal.GetCategoryByUrl, x => noredirect.If(x, url, _dal.Delete, _dal.Delete))
		.SwitchNotNull(_dal.GetProduct, x => noredirect.If(x, url, _dal.Delete, _dal.Delete))
		.SwitchNotNull(_dal.GetActionByUrl, x => noredirect.If(x, url, _dal.Delete, _dal.Delete))
		.SwitchNotNull(_dal.GetNewsByUrl, x => noredirect.If(x, url, _dal.Delete, _dal.Delete))
		.SwitchNotNull(_dal.GetProductCategoryByUrl, x => noredirect.If(x, url, _dal.Delete, _dal.Delete))
		.SwitchNotNull(_dal.GetProducer, x => noredirect.If(x, url, _dal.Delete, _dal.Delete))
		.SwitchDefault(x => false)
		.If<IActionResult>(() =>IsAjax.If(()=> Content("Удалено"), ()=> (IActionResult)RedirectPermanent($"{Request.Headers["Referer"]}")), ()=> Content("Удалить не получилось"));
		
		
# Минусы:
	1) В Visual Studio файл кода в 500 строк жутко тормозит. Но в Visual Code такой эффект не наблюдается.
	2) Затраты на сами вызовы функций.
	3) Сложная отладка.
# Плюсы:
	1) Отсутствие переменных
	2) Типизация
	3) Минимизация количества строк
	4) Очевидный путь выполнения функции.
	5) Асинхронный код выглядит так же как и синхронный. Достаточно лишь где-то в цепочке вызвать асинхронный метод.
# Советы:
	1) Нужно минимизировать функции, т.е. не пытаться "запихать" всю логику в одну функцию.
	2) Правильно определить "точку входа" - первую функцию. От этого зависит количество вызванных в дальнейшей цепочке функций.
		Пример:
		public int Count(IQuery query) => _client.Count(query).Convert(x => (int)x.Count);
		public int Count(IQuery query) => query.Convert(_client.Count).Convert(y => (int)y.Count);
	3) Стараться не изменять переданные аргументы, где этого можно избежать.
	4) Минимизировать количество лямда-выражений, вместо них подставлять функции.
	5) Не забывать про обработку ошибок (HasNotNullArg, ThrowIf).
