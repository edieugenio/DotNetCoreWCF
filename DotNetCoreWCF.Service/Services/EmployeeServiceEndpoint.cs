﻿using System.Threading.Tasks;
using DotNetCoreWCF.GrpcSample.Services;
using DotNetCoreWCF.Logic.Adapters;
using DotNetCoreWCF.Service.Core.Handlers;
using Grpc.Core;
using Unity;

namespace DotNetCoreWCF.Host.Services
{
	public class EmployeeServiceEndpoint : GrpcSample.Services.EmployeeService.EmployeeServiceBase
	{
		public EmployeeServiceEndpoint(IUnityContainer container)
		{
			Container = container;
		}

		private IUnityContainer Container { get; }

		public override Task<DeleteEmployeeResponse> Delete(DeleteEmployeeRequest request, ServerCallContext context)
		{
			using (var scope = Container.CreateChildContainer())
			{
				var handler = scope.Resolve<IDeleteEmployeeRequestHandler>();
				var requestAdapter = scope.Resolve<IDeleteEmployeeRequestAdapter>();
				var responseAdapter = scope.Resolve<IDeleteEmployeeResponseAdapter>();

				var employees = handler.Delete(requestAdapter.ToDomain(request));

				return Task.FromResult(responseAdapter.ToGrpc(employees));
			}
		}

		public override Task<EmployeeResponse> Get(EmployeeRequest request, ServerCallContext context)
		{
			using (var scope = Container.CreateChildContainer())
			{
				var handler = scope.Resolve<IGetEmployeeRequestHandler>();
				var employeeRequestAdapter = scope.Resolve<IEmployeeRequestAdapter>();
				var employeeResponseAdapter = scope.Resolve<IEmployeeResponseAdapter>();

				var employees = handler.Get(employeeRequestAdapter.ToDomain(request));

				return Task.FromResult(employeeResponseAdapter.ToGrpc(employees));
			}
		}

		public override Task<Employee> UpdateEmployee(Employee request, ServerCallContext context)
		{
			using (var scope = Container.CreateChildContainer())
			{
				var handler = scope.Resolve<IUpdateEmployeeRequestHandler>();
				var requestAdapter = scope.Resolve<IEmployeeAdapter>();

				var employees = handler.Update(requestAdapter.ToDomain(request));

				return Task.FromResult(requestAdapter.ToGrpc(employees));
			}
		}
	}
}