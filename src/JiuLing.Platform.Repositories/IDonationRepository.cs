﻿using JiuLing.Platform.Models.Entities;

namespace JiuLing.Platform.Repositories;

public interface IDonationRepository
{
    public Task<List<Donation>> GetAllAsync();
}