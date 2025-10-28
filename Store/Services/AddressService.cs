using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using Store.Data;
using Store.Services.IServices;

namespace Store.Services;

public class AddressService : Service<Address>, IAddressService
{
    public AddressService(ApplicationDbContext db, AutoMapper.IMapper mapper)
        : base(db, mapper)
    {
    }

    public async Task<ResponseData> GetByUserIdAsync(int userId)
    {
        try
        {
            var addresses = await _db.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            var addressDtos = _mapper.Map<List<AddressDTO>>(addresses);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Addresses retrieved successfully",
                addressDtos
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseData> GetByIdAsync(int id)
    {
        try
        {
            var address = await _db.Addresses.FindAsync(id);

            if (address == null)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Address not found",
                    null
                );
            }

            var addressDto = _mapper.Map<AddressDTO>(address);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Address retrieved successfully",
                addressDto
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseData> GetDefaultAddressAsync(int userId)
    {
        try
        {
            var address = await _db.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault == true);

            if (address == null)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Default address not found",
                    null
                );
            }

            var addressDto = _mapper.Map<AddressDTO>(address);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Default address retrieved successfully",
                addressDto
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseMessage> CreateAsync(int userId, AddressCreateDTO dto)
    {
        try
        {
            var address = _mapper.Map<Address>(dto);
            address.UserId = userId;
            address.CreatedAt = DateTime.Now;
            address.UpdatedAt = DateTime.Now;

            // If this is the first address or IsDefault is true, set as default
            var hasAddresses = await _db.Addresses.AnyAsync(a => a.UserId == userId);
            if (!hasAddresses || dto.IsDefault)
            {
                // Set all other addresses to not default
                var otherAddresses = await _db.Addresses
                    .Where(a => a.UserId == userId && a.IsDefault == true)
                    .ToListAsync();

                foreach (var addr in otherAddresses)
                {
                    addr.IsDefault = false;
                }

                address.IsDefault = true;
            }

            await _db.Addresses.AddAsync(address);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Address created successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> UpdateAsync(int id, int userId, AddressUpdateDTO dto)
    {
        try
        {
            var address = await _db.Addresses.FindAsync(id);

            if (address == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Address not found"
                );
            }

            // Check ownership
            if (address.UserId != userId)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.Forbidden,
                    false,
                    "You don't have permission to update this address"
                );
            }

            // Update fields
            address.ReceiverName = dto.ReceiverName ?? address.ReceiverName;
            address.PhoneNumber = dto.PhoneNumber ?? address.PhoneNumber;
            address.AddressLine1 = dto.AddressLine1 ?? address.AddressLine1;
            address.AddressLine2 = dto.AddressLine2 ?? address.AddressLine2;
            address.District = dto.District ?? address.District;
            address.Subdistrict = dto.Subdistrict ?? address.Subdistrict;
            address.Province = dto.Province ?? address.Province;
            address.PostalCode = dto.PostalCode ?? address.PostalCode;
            address.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Address updated successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> SetDefaultAsync(int userId, int addressId)
    {
        try
        {
            var address = await _db.Addresses.FindAsync(addressId);

            if (address == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Address not found"
                );
            }

            // Check ownership
            if (address.UserId != userId)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.Forbidden,
                    false,
                    "You don't have permission to modify this address"
                );
            }

            // Set all other addresses to not default
            var otherAddresses = await _db.Addresses
                .Where(a => a.UserId == userId && a.IsDefault == true && a.AddressId != addressId)
                .ToListAsync();

            foreach (var addr in otherAddresses)
            {
                addr.IsDefault = false;
            }

            // Set this address as default
            address.IsDefault = true;
            address.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Default address set successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> DeleteAsync(int id, int userId)
    {
        try
        {
            var address = await _db.Addresses.FindAsync(id);

            if (address == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Address not found"
                );
            }

            // Check ownership
            if (address.UserId != userId)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.Forbidden,
                    false,
                    "You don't have permission to delete this address"
                );
            }

            bool wasDefault = address.IsDefault == true;

            _db.Addresses.Remove(address);
            await _db.SaveChangesAsync();

            // If deleted address was default, set another address as default
            if (wasDefault)
            {
                var firstAddress = await _db.Addresses
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync();

                if (firstAddress != null)
                {
                    firstAddress.IsDefault = true;
                    await _db.SaveChangesAsync();
                }
            }

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Address deleted successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }
}
