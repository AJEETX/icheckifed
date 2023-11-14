using Microsoft.AspNetCore.Identity;

using risk.control.system.AppConstant;
using risk.control.system.Data;
using risk.control.system.Models;
using risk.control.system.Services;

namespace risk.control.system.Seeds
{
    public static class DatabaseSeed
    {
        public static async Task SeedDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var httpClientService = scope.ServiceProvider.GetRequiredService<IHttpClientService>();
            var vendorUserManager = scope.ServiceProvider.GetRequiredService<UserManager<VendorApplicationUser>>();
            var clientUserManager = scope.ServiceProvider.GetRequiredService<UserManager<ClientCompanyApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            context.Database.EnsureCreated();

            //check for users
            if (context.ApplicationUser.Any())
            {
                //Do nothing
                return; //if user is not empty, DB has been seed
            }

            //CREATE ROLES
            else
            {
                await roleManager.CreateAsync(new ApplicationRole(AppRoles.PortalAdmin.ToString().Substring(0, 2).ToUpper(), AppRoles.PortalAdmin.ToString()));
                await roleManager.CreateAsync(new ApplicationRole(AppRoles.CompanyAdmin.ToString().Substring(0, 2).ToUpper(), AppRoles.CompanyAdmin.ToString()));
                await roleManager.CreateAsync(new ApplicationRole(AppRoles.AgencyAdmin.ToString().Substring(0, 2).ToUpper(), AppRoles.AgencyAdmin.ToString()));
                await roleManager.CreateAsync(new ApplicationRole(AppRoles.Creator.ToString().Substring(0, 2).ToUpper(), AppRoles.Creator.ToString()));
                await roleManager.CreateAsync(new ApplicationRole(AppRoles.Assigner.ToString().Substring(0, 2).ToUpper(), AppRoles.Assigner.ToString()));
                await roleManager.CreateAsync(new ApplicationRole(AppRoles.Assessor.ToString().Substring(0, 2).ToUpper(), AppRoles.Assessor.ToString()));
                await roleManager.CreateAsync(new ApplicationRole(AppRoles.Supervisor.ToString().Substring(0, 2).ToUpper(), AppRoles.Supervisor.ToString()));
                await roleManager.CreateAsync(new ApplicationRole(AppRoles.Agent.ToString().Substring(0, 2).ToUpper(), AppRoles.Agent.ToString()));
            }

            var country = context.Country.FirstOrDefault(c => c.Code == Applicationsettings.INDIA_CODE);
            string countryId = string.Empty;
            if (country == null)
            {
                var india = new Country
                {
                    Name = Applicationsettings.INDIA_NAME,
                    Code = Applicationsettings.INDIA_CODE,
                };

                var indiaCountry = await context.Country.AddAsync(india);
                countryId = indiaCountry.Entity.CountryId;
                await PinCodeStateSeed.SeedPincode(context, indiaCountry.Entity);
                await context.SaveChangesAsync(null, false);
            }
            else
            {
                countryId = country.CountryId;
            }

            #region LINE OF BUSINESS

            var claimLob = context.LineOfBusiness.FirstOrDefault(l => l.Name == Applicationsettings.CLAIM_LOB);
            string claimLobName = string.Empty;
            if (claimLob == null)
            {
                var claims = new LineOfBusiness
                {
                    Name = Applicationsettings.CLAIM_LOB,
                    Code = Applicationsettings.CLAIM_LOB,
                    MasterData = true,
                };

                var claimCaseType = await context.LineOfBusiness.AddAsync(claims);
                claimLobName = claimCaseType.Entity.LineOfBusinessId;
            }
            else
            {
                claimLobName = claimLob.LineOfBusinessId;
            }

            var underwritingLob = context.LineOfBusiness.FirstOrDefault(l => l.Name == Applicationsettings.UNDERWRITING_LOB);
            string underwritingLobName = string.Empty;
            if (underwritingLob == null)
            {
                var underwriting = new LineOfBusiness
                {
                    Name = Applicationsettings.UNDERWRITING_LOB,
                    Code = Applicationsettings.UNDERWRITING_LOB,
                    MasterData = true,
                };

                var underwritingCaseType = await context.LineOfBusiness.AddAsync(underwriting);
                underwritingLobName = underwritingCaseType.Entity.LineOfBusinessId;
            }
            else
            {
                underwritingLobName = underwritingLob.LineOfBusinessId;
            }

            #endregion LINE OF BUSINESS

            #region INVESTIGATION SERVICE TYPES

            var compService = context.InvestigationServiceType.FirstOrDefault(i => i.Code == Applicationsettings.INVESTIGATION_TYPE.COMPREHENSIVE_CODE);
            var claimComprehensive = new InvestigationServiceType
            {
                Name = Applicationsettings.INVESTIGATION_TYPE.COMPREHENSIVE,
                Code = Applicationsettings.INVESTIGATION_TYPE.COMPREHENSIVE_CODE,
                MasterData = true,
                LineOfBusinessId = claimLobName
            };
            var claimComprehensiveService = await context.InvestigationServiceType.AddAsync(claimComprehensive);

            var claimNonComprehensive = new InvestigationServiceType
            {
                Name = Applicationsettings.INVESTIGATION_TYPE.NON_COMPREHENSIVE,
                Code = Applicationsettings.INVESTIGATION_TYPE.NON_COMPREHENSIVE_CODE,
                MasterData = true,
                LineOfBusinessId = claimLobName
            };

            var claimNonComprehensiveService = await context.InvestigationServiceType.AddAsync(claimNonComprehensive);

            var claimDocumentCollection = new InvestigationServiceType
            {
                Name = Applicationsettings.INVESTIGATION_TYPE.DOCUMENT_COLLECTION,
                Code = Applicationsettings.INVESTIGATION_TYPE.DOCUMENT_COLLECTION_CODE,
                MasterData = true,
                LineOfBusinessId = claimLobName
            };

            var claimDocumentCollectionService = await context.InvestigationServiceType.AddAsync(claimDocumentCollection);

            var claimDiscreet = new InvestigationServiceType
            {
                Name = Applicationsettings.INVESTIGATION_TYPE.DISCREET,
                Code = Applicationsettings.INVESTIGATION_TYPE.DISCREET_CODE,
                MasterData = true,
                LineOfBusinessId = claimLobName
            };

            var claimDiscreetService = await context.InvestigationServiceType.AddAsync(claimDiscreet);

            var underWritingPreVerification = new InvestigationServiceType
            {
                Name = "PRE-ONBOARDING-VERIFICATION",
                Code = "PRE-OV",
                MasterData = true,
                LineOfBusinessId = underwritingLobName
            };

            var underWritingPreVerificationService = await context.InvestigationServiceType.AddAsync(underWritingPreVerification);

            var underWritingPostVerification = new InvestigationServiceType
            {
                Name = "POST-ONBOARDING-VERIFICATION",
                Code = "POST-OV",
                MasterData = true,
                LineOfBusinessId = underwritingLobName
            };

            var underWritingPostVerificationService = await context.InvestigationServiceType.AddAsync(underWritingPostVerification);

            #endregion INVESTIGATION SERVICE TYPES

            #region //CREATE RISK CASE DETAILS

            //CASE STATUS

            var initiated = new InvestigationCaseStatus
            {
                Name = CONSTANTS.CASE_STATUS.INITIATED,
                Code = CONSTANTS.CASE_STATUS.INITIATED,
                MasterData = true,
            };

            var initiatedStatus = await context.InvestigationCaseStatus.AddAsync(initiated);

            var inProgress = new InvestigationCaseStatus
            {
                Name = CONSTANTS.CASE_STATUS.INPROGRESS,
                Code = CONSTANTS.CASE_STATUS.INPROGRESS,
                MasterData = true,
            };

            var inProgressStatus = await context.InvestigationCaseStatus.AddAsync(inProgress);

            var finished = new InvestigationCaseStatus
            {
                Name = CONSTANTS.CASE_STATUS.FINISHED,
                Code = CONSTANTS.CASE_STATUS.FINISHED,
                MasterData = true,
            };

            var finishedStatus = await context.InvestigationCaseStatus.AddAsync(finished);

            //CASE SUBSTATUS

            var created = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.CREATED_BY_CREATOR,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.CREATED_BY_CREATOR,
                MasterData = true,
                InvestigationCaseStatusId = initiatedStatus.Entity.InvestigationCaseStatusId
            };
            var createdSubStatus = await context.InvestigationCaseSubStatus.AddAsync(created);

            var edited = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.EDITED_BY_CREATOR,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.EDITED_BY_CREATOR,
                MasterData = true,
                InvestigationCaseStatusId = initiatedStatus.Entity.InvestigationCaseStatusId
            };
            var editedSubStatus = await context.InvestigationCaseSubStatus.AddAsync(edited);

            var assigned = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.ASSIGNED_TO_ASSIGNER,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.ASSIGNED_TO_ASSIGNER,
                MasterData = true,
                InvestigationCaseStatusId = inProgressStatus.Entity.InvestigationCaseStatusId
            };

            var assignedSubStatus = await context.InvestigationCaseSubStatus.AddAsync(assigned);

            var allocated = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.ALLOCATED_TO_VENDOR,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.ALLOCATED_TO_VENDOR,
                MasterData = true,
                InvestigationCaseStatusId = inProgressStatus.Entity.InvestigationCaseStatusId
            };

            var allocatedSubStatus = await context.InvestigationCaseSubStatus.AddAsync(allocated);

            var assignedToAgent = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.ASSIGNED_TO_AGENT,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.ASSIGNED_TO_AGENT,
                MasterData = true,
                InvestigationCaseStatusId = inProgressStatus.Entity.InvestigationCaseStatusId
            };

            var assignedToAgentSubStatus = await context.InvestigationCaseSubStatus.AddAsync(assignedToAgent);

            var submittedtoSupervisor = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.SUBMITTED_TO_SUPERVISOR,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.SUBMITTED_TO_SUPERVISOR,
                MasterData = true,
                InvestigationCaseStatusId = inProgressStatus.Entity.InvestigationCaseStatusId
            };

            var submittedtoSupervisorSubStatus = await context.InvestigationCaseSubStatus.AddAsync(submittedtoSupervisor);

            var submittedToAssessor = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.SUBMITTED_TO_ASSESSOR,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.SUBMITTED_TO_ASSESSOR,
                MasterData = true,
                InvestigationCaseStatusId = inProgressStatus.Entity.InvestigationCaseStatusId
            };

            var submittedToAssessorSubStatus = await context.InvestigationCaseSubStatus.AddAsync(submittedToAssessor);

            var approved = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.APPROVED_BY_ASSESSOR,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.APPROVED_BY_ASSESSOR,
                MasterData = true,
                InvestigationCaseStatusId = finishedStatus.Entity.InvestigationCaseStatusId
            };

            var approvededSubStatus = await context.InvestigationCaseSubStatus.AddAsync(approved);

            var rejected = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.REJECTED_BY_ASSESSOR,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.REJECTED_BY_ASSESSOR,
                MasterData = true,
                InvestigationCaseStatusId = finishedStatus.Entity.InvestigationCaseStatusId
            };

            var rejectedSubStatus = await context.InvestigationCaseSubStatus.AddAsync(rejected);

            var reassigned = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.REASSIGNED_TO_ASSIGNER,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.REASSIGNED_TO_ASSIGNER,
                MasterData = true,
                InvestigationCaseStatusId = finishedStatus.Entity.InvestigationCaseStatusId
            };
            var acceptedSubStatus = await context.InvestigationCaseSubStatus.AddAsync(reassigned);

            var withdrawnByCompany = new InvestigationCaseSubStatus
            {
                Name = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.WITHDRAWN_BY_COMPANY,
                Code = CONSTANTS.CASE_STATUS.CASE_SUBSTATUS.WITHDRAWN_BY_COMPANY,
                MasterData = true,
                InvestigationCaseStatusId = finishedStatus.Entity.InvestigationCaseStatusId
            };

            var withdrawnByCompanySubStatus = await context.InvestigationCaseSubStatus.AddAsync(withdrawnByCompany);

            #endregion //CREATE RISK CASE DETAILS

            #region MISCELLANEOUS SETUP

            await ClientCompanySetupSeed.Seed(context);

            #endregion MISCELLANEOUS SETUP

            #region CLIENT/ VENDOR COMPANY

            var (checker, verify, investigate, clientCompanyId, clientCompanyId2) = await ClientVendorSeed.Seed(context, countryId,
                claimComprehensiveService.Entity.InvestigationServiceTypeId, claimDiscreetService.Entity,
                claimDocumentCollectionService.Entity, claimLobName, httpClientService);

            #endregion CLIENT/ VENDOR COMPANY

            #region APPLICATION USERS ROLES

            await PortalAdminSeed.Seed(context, countryId, userManager, roleManager);

            await ClientApplicationUserSeed.Seed(context, countryId, clientUserManager, clientCompanyId);

            await ClientApplicationUserSeed.Seed(context, countryId, clientUserManager, clientCompanyId2);

            await VendorApplicationUserSeed.Seed(context, countryId, vendorUserManager, checker);

            await VendorApplicationUserSeed.Seed(context, countryId, vendorUserManager, verify);

            await VendorApplicationUserSeed.Seed(context, countryId, vendorUserManager, investigate);

            await context.SaveChangesAsync(null, false);

            #endregion APPLICATION USERS ROLES
        }
    }
}