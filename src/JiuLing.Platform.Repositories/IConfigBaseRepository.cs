﻿namespace JiuLing.Platform.Repositories;

public interface IConfigBaseRepository
{
    public Task<T?> GetOneAsync<T>(string key);
    public Task<string> GetOneAsync(string key);
}