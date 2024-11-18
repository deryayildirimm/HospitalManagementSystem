﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class EfCoreDoctorLeaveRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
  : EfCoreRepository<HealthCareDbContext, DoctorLeave, Guid>(dbContextProvider), IDoctorLeaveRepository
{
        public virtual async Task DeleteAllAsync(
            string? filterText = null,
            Guid? doctorId = null,
            DateTime? startDateMin = null,
            DateTime? startDateMax = null,
            DateTime? endDateMin = null,
            DateTime? endDateMax = null,
            string? reason = null,
            CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        query = ApplyFilter(query, filterText , doctorId, startDateMin, startDateMax, endDateMin, endDateMax, reason);

        var ids = query.Select(x => x.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<DoctorLeave>> GetListAsync(
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText,doctorId, startDateMin, startDateMax, endDateMin, endDateMax, reason);
      //  query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PatientConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText,doctorId, startDateMin, startDateMax, endDateMin, endDateMax, reason );
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    // kotu kod sımdılık ıdare edıverın duzenlıycem calısırsa 
    public virtual async Task<List<DoctorLeave>> GetListByDoctorNumberAsync(
        Guid? doctorId,
        string? identityNumber,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        // DoctorId varsa kullan
        if (doctorId.HasValue)
        {
            return await dbContext.DoctorLeaves
                .Where(dl => dl.DoctorId == doctorId.Value)
                .ToListAsync(cancellationToken);
        }
        
        // InsuranceNumber ile doktor bulunup DoctorId üzerinden sorgula
        if (!string.IsNullOrWhiteSpace(identityNumber))
        {
            var doctor = await dbContext.Doctors
                .FirstOrDefaultAsync(d => d.IdentityNumber == identityNumber, cancellationToken);

            if (doctor == null)
            {
                throw new BusinessException("DoctorNotFound")
                    .WithData("InsuranceNumber", identityNumber);
            }

            return await dbContext.DoctorLeaves
                .Where(dl => dl.DoctorId == doctor.Id)
                .ToListAsync(cancellationToken);
        }

        throw new BusinessException("InvalidRequest")
            .WithData("Message", "Either DoctorId or IdentityNumber must be provided.");
        
        /*
        // DoctorId varsa sorgulama yap; yoksa IdentityNumber ile doktor bul
        // doctorId varsa kullan, yoksa identityNumber ile doktorId bul
        var doctorIdToQuery = doctorId 
                              ?? (await dbContext.Doctors
                                  .Where(d => d.IdentityNumber == identityNumber)
                                  .Select(d => (Guid?)d.Id)
                                  .FirstOrDefaultAsync(cancellationToken))
                              ?? throw new BusinessException("DoctorNotFound")
                                  .WithData("IdentityNumber", identityNumber ?? "null");

        // DoctorId'ye göre DoctorLeave sorgula
        return await dbContext.DoctorLeaves
            .Where(dl => dl.DoctorId == doctorIdToQuery)
            .ToListAsync(cancellationToken);
        */
    }
    protected virtual IQueryable<DoctorLeave> ApplyFilter(
        IQueryable<DoctorLeave> query,
        string? filterText = null,
        Guid? doctorId = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        DateTime? endDateMin = null,
        DateTime? endDateMax = null,
        string? reason = null)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                e => e.Reason!.Contains(filterText!))
            .WhereIf(!string.IsNullOrWhiteSpace(reason), e => e.Reason!.ToLower().Contains(reason!.ToLower()))
            .WhereIf(startDateMin.HasValue, e => e.StartDate >= startDateMin!.Value)
            .WhereIf(startDateMax.HasValue, e => e.StartDate <= startDateMax!.Value)
            .WhereIf(endDateMin.HasValue, e => e.EndDate >= endDateMin!.Value)
            .WhereIf(endDateMax.HasValue, e => e.EndDate <= endDateMax!.Value)
            .WhereIf(doctorId.HasValue, e => e.DoctorId == doctorId);

    }
}