using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using MimeKit.Encodings;

using risk.control.system.AppConstant;
using risk.control.system.Data;
using risk.control.system.Models;
using risk.control.system.Models.ViewModel;
using risk.control.system.Services;

namespace risk.control.system.Seeds
{
    public class ClientVendorSeed
    {
        private static string currentPinCode = "515631";
        private static string currentDistrict = "ANANTAPUR";
        private static string currentState = "AD";
        private static string biharPincode = "853204";

        public static async Task<(Vendor checker, Vendor verify, Vendor investigate, string clientCompanyId, string clientCompanyId2)> Seed(ApplicationDbContext context, string indiaCountryId,
            string investigationServiceTypeId, InvestigationServiceType discreetServiceType, InvestigationServiceType docServiceType, string lineOfBusinessId, IHttpClientService httpClientService)
        {
            var companyPinCode = context.PinCode.Include(p => p.District).FirstOrDefault(s => s.Code == Applicationsettings.CURRENT_PINCODE);
            var companyDistrict = context.District.Include(d => d.State).FirstOrDefault(s => s.DistrictId == companyPinCode.District.DistrictId);
            var companyStateId = context.State.FirstOrDefault(s => s.Code.StartsWith(Applicationsettings.CURRENT_STATE))?.StateId ?? default!;

            var pinCodeData = await httpClientService.GetPinCodeLatLng(companyPinCode.Code);

            companyPinCode.Latitude = pinCodeData.FirstOrDefault()?.Lat.ToString();
            companyPinCode.Longitude = pinCodeData.FirstOrDefault()?.Lng.ToString();

            //CREATE VENDOR COMPANY
            var checkerExists = context.Vendor.Any(v => v.Email == Applicationsettings.AGENCY1DOMAIN);
            var checker = new Vendor
            {
                Name = Applicationsettings.AGENCY1NAME,
                Addressline = "1, Nice Road  ",
                Branch = "MAHATTAN",
                Code = Applicationsettings.AGENCY1CODE,
                ActivatedDate = DateTime.Now,
                AgreementDate = DateTime.Now,
                BankName = "WESTPAC",
                BankAccountNumber = "1234567",
                IFSCCode = "IFSC100",
                CountryId = indiaCountryId,
                DistrictId = companyDistrict.DistrictId,
                StateId = companyStateId,
                PinCodeId = companyPinCode.PinCodeId,
                Description = "HEAD OFFICE ",
                Email = Applicationsettings.AGENCY1DOMAIN,
                PhoneNumber = "8888004739",
                DocumentUrl = "/img/checker.png"
            };
            if (!checkerExists)
            {
                var checkerAgency = await context.Vendor.AddAsync(checker);
                var checkerSericesWithPinCodes = new List<VendorInvestigationServiceType>
            {
                new VendorInvestigationServiceType{
                    VendorId = checkerAgency.Entity.VendorId,
                    InvestigationServiceTypeId = investigationServiceTypeId,
                    Price = 199,
                    StateId = context.State.FirstOrDefault(s => s.Code.StartsWith(currentState))?.StateId ?? default!,
                    DistrictId = context.District.FirstOrDefault(s => s.Name == Applicationsettings.CURRENT_DISTRICT)?.DistrictId ?? default!,
                    LineOfBusinessId = lineOfBusinessId,
                    CountryId = indiaCountryId,
                    PincodeServices = new List<ServicedPinCode>
                    {
                        new ServicedPinCode
                        {
                            Pincode = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Code ?? default !,
                            Name = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Name ?? default !
                        }
                    }
                },
                new VendorInvestigationServiceType{
                    VendorId = checkerAgency.Entity.VendorId,
                    InvestigationServiceTypeId = docServiceType.InvestigationServiceTypeId,
                    Price = 99,
                    StateId = context.State.FirstOrDefault(s => s.Code.StartsWith(currentState))?.StateId ?? default!,
                    DistrictId = context.District.FirstOrDefault(s => s.Name == Applicationsettings.CURRENT_DISTRICT)?.DistrictId ?? default!,
                    LineOfBusinessId = lineOfBusinessId,
                    CountryId = indiaCountryId,
                    PincodeServices = new List<ServicedPinCode>
                    {
                        new ServicedPinCode
                        {
                            Pincode = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Code ?? default !,
                            Name = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Name ?? default !
                        }
                    }
                }
            };
                checker.VendorInvestigationServiceTypes = checkerSericesWithPinCodes;
            }
            var verifyExists = context.Vendor.Any(v => v.Email == Applicationsettings.AGENCY2DOMAIN);

            var verify = new Vendor
            {
                Name = Applicationsettings.AGENCY2NAME,
                Addressline = "10, Clear Road  ",
                Branch = "BLACKBURN",
                Code = Applicationsettings.AGENCY2CODE,
                ActivatedDate = DateTime.Now,
                AgreementDate = DateTime.Now,
                BankName = "SBI BANK",
                BankAccountNumber = "9876543",
                IFSCCode = "IFSC999",
                CountryId = indiaCountryId,
                DistrictId = companyDistrict.DistrictId,
                StateId = companyStateId,
                PinCodeId = companyPinCode.PinCodeId,
                Description = "HEAD OFFICE ",
                Email = Applicationsettings.AGENCY2DOMAIN,
                PhoneNumber = "4444404739",
                DocumentUrl = "/img/verify.png"
            };
            if (!verifyExists)
            {
                var verifyAgency = await context.Vendor.AddAsync(verify);
                var verifySericesWithPinCodes = new List<VendorInvestigationServiceType>
            {
                new VendorInvestigationServiceType{
                    VendorId = verifyAgency.Entity.VendorId,
                    InvestigationServiceTypeId = investigationServiceTypeId,
                    Price = 399,
                    StateId = context.State.FirstOrDefault(s => s.Code.StartsWith(currentState))?.StateId ?? default!,
                    DistrictId = context.District.FirstOrDefault(s => s.Name == Applicationsettings.CURRENT_DISTRICT)?.DistrictId ?? default!,
                    CountryId = indiaCountryId,
                    LineOfBusinessId = lineOfBusinessId,
                    PincodeServices = new List<ServicedPinCode>
                    {
                        new ServicedPinCode
                        {
                            Pincode = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Code ?? default !,
                            Name = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Name ?? default !
                        }
                    }
                },
                new VendorInvestigationServiceType{
                    VendorId = verifyAgency.Entity.VendorId,
                    InvestigationServiceTypeId = discreetServiceType.InvestigationServiceTypeId,
                    Price = 299,
                    StateId = context.State.FirstOrDefault(s => s.Code.StartsWith(currentState))?.StateId ?? default!,
                    DistrictId = context.District.FirstOrDefault(s => s.Name == Applicationsettings.CURRENT_DISTRICT)?.DistrictId ?? default!,
                    CountryId = indiaCountryId,
                    LineOfBusinessId = lineOfBusinessId,
                    PincodeServices = new List<ServicedPinCode>
                    {
                        new ServicedPinCode
                        {
                            Pincode = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Code ?? default !,
                            Name = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Name ?? default !
                        }
                    }
                }
            };
                verify.VendorInvestigationServiceTypes = verifySericesWithPinCodes;
            }

            var investigate = new Vendor
            {
                Name = Applicationsettings.AGENCY3NAME,
                Addressline = "1, Main Road  ",
                Branch = "CLAYTON",
                Code = Applicationsettings.AGENCY3CODE,
                ActivatedDate = DateTime.Now,
                AgreementDate = DateTime.Now,
                BankName = "HDFC BANK",
                BankAccountNumber = "9876543",
                IFSCCode = "IFSC999",
                CountryId = indiaCountryId,
                DistrictId = companyDistrict.DistrictId,
                StateId = companyStateId,
                PinCodeId = companyPinCode.PinCodeId,
                Description = "HEAD OFFICE ",
                Email = Applicationsettings.AGENCY3DOMAIN,
                PhoneNumber = "7964404160",
                DocumentUrl = "/img/investigate.png"
            };
            var investigateExists = context.Vendor.Any(v => v.Email == Applicationsettings.AGENCY3DOMAIN);
            if (!investigateExists)
            {
                var investigateAgency = await context.Vendor.AddAsync(investigate);
                var investigateSericesWithPinCodes = new List<VendorInvestigationServiceType>
            {
                new VendorInvestigationServiceType{
                    VendorId = investigateAgency.Entity.VendorId,
                    InvestigationServiceTypeId = docServiceType.InvestigationServiceTypeId,
                    Price = 199,
                    StateId = context.State.FirstOrDefault(s => s.Code.StartsWith(currentState))?.StateId ?? default!,
                    DistrictId = context.District.FirstOrDefault(s => s.Name == Applicationsettings.CURRENT_DISTRICT)?.DistrictId ?? default!,
                    CountryId = indiaCountryId,
                    LineOfBusinessId = lineOfBusinessId,
                    PincodeServices = new List<ServicedPinCode>
                    {
                        new ServicedPinCode
                        {
                            Pincode = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Code ?? default !,
                            Name = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Name ?? default !
                        }
                    }
                },
                new VendorInvestigationServiceType{
                    VendorId = investigateAgency.Entity.VendorId,
                    InvestigationServiceTypeId = discreetServiceType.InvestigationServiceTypeId,
                    Price = 299,
                    StateId = context.State.FirstOrDefault(s => s.Code.StartsWith(currentState))?.StateId ?? default!,
                    DistrictId = context.District.FirstOrDefault(s => s.Name == Applicationsettings.CURRENT_DISTRICT)?.DistrictId ?? default!,
                    CountryId = indiaCountryId,
                    LineOfBusinessId = lineOfBusinessId,
                    PincodeServices = new List<ServicedPinCode>
                    {
                        new ServicedPinCode
                        {
                            Pincode = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Code ?? default !,
                            Name = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Name ?? default !
                        }
                    }
                },
                new VendorInvestigationServiceType{
                    VendorId = investigateAgency.Entity.VendorId,
                    InvestigationServiceTypeId = investigationServiceTypeId,
                    Price = 599,
                    StateId = context.State.FirstOrDefault(s => s.Code.StartsWith(currentState))?.StateId ?? default!,
                    DistrictId = context.District.FirstOrDefault(s => s.Name == Applicationsettings.CURRENT_DISTRICT)?.DistrictId ?? default!,
                    CountryId = indiaCountryId,
                    LineOfBusinessId = lineOfBusinessId,
                    PincodeServices = new List<ServicedPinCode>
                    {
                        new ServicedPinCode
                        {
                            Pincode = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Code ?? default !,
                            Name = context.PinCode.FirstOrDefault(s => s.Code == currentPinCode)?.Name ?? default !
                        }
                    }
                }
            };
                investigate.VendorInvestigationServiceTypes = investigateSericesWithPinCodes;
            }

            //CREATE COMPANY
            var canaraHsbc = new ClientCompany
            {
                ClientCompanyId = Guid.NewGuid().ToString(),
                Name = Applicationsettings.COMPANYNAME,
                Addressline = "100 GOOD STREET ",
                Branch = "FOREST HILL CHASE",
                Code = Applicationsettings.COMPANYCODE,
                ActivatedDate = DateTime.Now,
                AgreementDate = DateTime.Now,
                BankName = "NAB",
                BankAccountNumber = "1234567",
                IFSCCode = "IFSC100",
                CountryId = indiaCountryId,
                DistrictId = companyDistrict.DistrictId,
                StateId = companyStateId,
                PinCodeId = companyPinCode.PinCodeId,
                Description = "CORPORATE OFFICE ",
                Email = Applicationsettings.COMPANYDOMAIN,
                DocumentUrl = "/img/chl.png",
                PhoneNumber = "9988004739",
                EmpanelledVendors = new List<Vendor> { checker, verify, investigate }
            };

            var hdfc = new ClientCompany
            {
                ClientCompanyId = Guid.NewGuid().ToString(),
                Name = Applicationsettings.COMPANY2NAME,
                Addressline = "999 NEXT STREET ",
                Branch = "BLACKBURN ROAD",
                Code = Applicationsettings.COMPANY2CODE,
                ActivatedDate = DateTime.Now,
                AgreementDate = DateTime.Now,
                BankName = "SBI",
                BankAccountNumber = "987654",
                IFSCCode = "IFSC999",
                CountryId = indiaCountryId,
                DistrictId = companyDistrict.DistrictId,
                StateId = companyStateId,
                PinCodeId = companyPinCode.PinCodeId,
                Description = "BRANCH OFFICE ",
                Email = Applicationsettings.COMPANY2DOMAIN,
                DocumentUrl = "/img/HDFC.jpg",
                PhoneNumber = "9999999999",
                EmpanelledVendors = new List<Vendor> { checker, verify, investigate }
            };
            var companyCanaraExists = context.ClientCompany.Any(v => v.Email == Applicationsettings.COMPANYDOMAIN);
            var companyHdfcExists = context.ClientCompany.Any(v => v.Email == Applicationsettings.COMPANY2DOMAIN);
            string canaraCompanyId = string.Empty;
            string hdfcCompanyId = string.Empty;
            if (!companyCanaraExists)
            {
                var insuranceCompany = await context.ClientCompany.AddAsync(canaraHsbc);
                canaraCompanyId = insuranceCompany.Entity.ClientCompanyId;
                checker.Clients.Add(insuranceCompany.Entity);
                verify.Clients.Add(insuranceCompany.Entity);
                investigate.Clients.Add(insuranceCompany.Entity);
            }
            if (!companyHdfcExists)
            {
                var insuranceCompany = await context.ClientCompany.AddAsync(hdfc);
                hdfcCompanyId = insuranceCompany.Entity.ClientCompanyId;
                checker.Clients.Add(insuranceCompany.Entity);
                verify.Clients.Add(insuranceCompany.Entity);
                investigate.Clients.Add(insuranceCompany.Entity);
            }

            await context.SaveChangesAsync(null, false);
            return (checker, verify, investigate, canaraCompanyId, hdfcCompanyId);
        }
    }
}