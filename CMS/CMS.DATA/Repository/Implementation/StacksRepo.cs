using CMS.DATA.Repository.RepositoryInterface;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CMS.DATA.Context;
using CMS.DATA.DTO;
using CMS.DATA.Entities;
using CMS.DATA.Repository.RepositoryInterface;
using Microsoft.EntityFrameworkCore;

namespace CMS.DATA.Repository.Implementation
{
	public class StacksRepo : IStacksRepo
	{
		private readonly CMSDbContext _context;
		private readonly IMapper _mapper;

		public StacksRepo(CMSDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<Stack> GetStackAsync(string stackid)
		{
			return await _context.Stacks.FindAsync(stackid);
		}
		public async Task<IEnumerable<Stack>> GetStacks()
		{
			var stacks = await _context.Stacks.ToListAsync();
			return stacks;
		}


		public async Task<bool?> UpdateStackbyId(string stackId, Stack entity)
		{
			var existingStack = await _context.Stacks.FindAsync(stackId);
			if (existingStack == null)
			{
				return null;
			}
			existingStack.StackName = entity.StackName;
			existingStack.DateUpdated = DateTime.UtcNow;
			_context.Stacks.Update(existingStack);
			var updateResult = await _context.SaveChangesAsync();
			if (updateResult > 0)
			{
				return true;
			}
			return false;
		}
			public async Task<List<UserDto>> GetUsersByStack(string stackId)
			{
				var userStacks = await _context.UserStack
					.Include(us => us.User)
					.Where(us => us.StackId == stackId)
					.Select(us => us.User)
					.ToListAsync();
				return _mapper.Map<List<UserDto>>(userStacks);
			}

			public async Task<bool> DeleteStack(string stackId)
			{
				var stack = await _context.Stacks.FindAsync(stackId);
				if (stack == null)
				{
					return false;
				}
				_context.Stacks.Remove(stack);
				var result = await _context.SaveChangesAsync();
				if (result > 0)
				{
					return true;
				}
				throw new Exception("stack not deleted");
			}
		}
	}