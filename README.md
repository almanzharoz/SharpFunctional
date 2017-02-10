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
