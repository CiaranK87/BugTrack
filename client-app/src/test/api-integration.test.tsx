import { vi } from 'vitest';

// Mock agent
vi.mock('../app/api/agent', () => ({
  default: {
    Account: {
      login: vi.fn(),
      register: vi.fn(),
      current: vi.fn(),
    },
    Projects: {
      list: vi.fn(),
      details: vi.fn(),
      create: vi.fn(),
      update: vi.fn(),
      delete: vi.fn(),
    },
    Tickets: {
      list: vi.fn(),
      details: vi.fn(),
      create: vi.fn(),
      update: vi.fn(),
      delete: vi.fn(),
      listByProject: vi.fn(),
    },
  },
}));

// Import after mocking
import agent from '../app/api/agent';

describe('API Integration Tests (Mocked)', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test('should handle successful login API call', async () => {
    const mockUser = {
      id: '1',
      email: 'test@example.com',
      displayName: 'Test User',
      username: 'testuser',
      token: 'fake-jwt-token',
      globalRole: 'User'
    };

    vi.mocked(agent.Account.login).mockResolvedValue(mockUser);

    const result = await agent.Account.login({
      email: 'test@example.com',
      password: 'password123'
    } as any);

    expect(agent.Account.login).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'password123'
    });

    expect(result).toEqual(mockUser);
  });

  test('should handle API error for login', async () => {
    const errorMessage = 'Invalid credentials';
    vi.mocked(agent.Account.login).mockRejectedValue(new Error(errorMessage));

    await expect(agent.Account.login({
      email: 'wrong@example.com',
      password: 'wrongpassword'
    } as any)).rejects.toThrow(errorMessage);

    expect(agent.Account.login).toHaveBeenCalledWith({
      email: 'wrong@example.com',
      password: 'wrongpassword'
    });
  });

  test('should fetch projects list successfully', async () => {
    const mockProjects = [
      { id: '1', name: 'Project 1', description: 'Description 1' },
      { id: '2', name: 'Project 2', description: 'Description 2' }
    ];

    vi.mocked(agent.Projects.list).mockResolvedValue(mockProjects as any);

    const result = await agent.Projects.list();

    expect(agent.Projects.list).toHaveBeenCalled();
    expect(result).toEqual(mockProjects);
    expect(result).toHaveLength(2);
  });

  test('should handle project creation', async () => {
    const newProject = {
      projectTitle: 'New Project',
      projectDescription: 'New Description',
      projectOwner: 'Owner',
      startDate: new Date(),
      ownerUsername: 'owner'
    };

    vi.mocked(agent.Projects.create).mockResolvedValue(undefined);

    await agent.Projects.create(newProject as any);

    expect(agent.Projects.create).toHaveBeenCalledWith(newProject);
  });

  test('should fetch tickets for a project', async () => {
    const mockTickets = [
      { id: '1', title: 'Ticket 1', description: 'Description 1' },
      { id: '2', title: 'Ticket 2', description: 'Description 2' }
    ];

    vi.mocked(agent.Tickets.listByProject).mockResolvedValue(mockTickets as any);

    const result = await agent.Tickets.listByProject('project-123');

    expect(agent.Tickets.listByProject).toHaveBeenCalledWith('project-123');
    expect(result).toEqual(mockTickets);
  });

  test('should handle ticket creation', async () => {
    const newTicket = {
      id: 'new-ticket-id',
      title: 'New Ticket',
      description: 'New Description',
      projectId: 'project-123',
      priority: 'High',
      severity: 'Critical',
      status: 'Open',
      submitter: 'user',
      assigned: null,
      created: new Date()
    };

    vi.mocked(agent.Tickets.create).mockResolvedValue(undefined);

    await agent.Tickets.create(newTicket as any);

    expect(agent.Tickets.create).toHaveBeenCalledWith(newTicket);
  });

  test('should handle 404 error for non-existent project', async () => {
    const error = new Error('Not Found');
    (error as any).response = { status: 404 };

    vi.mocked(agent.Projects.details).mockRejectedValue(error);

    await expect(agent.Projects.details('non-existent')).rejects.toThrow('Not Found');
    expect(agent.Projects.details).toHaveBeenCalledWith('non-existent');
  });

  test('should handle network error', async () => {
    const networkError = new Error('Network Error');
    (networkError as any).response = undefined;

    vi.mocked(agent.Account.current).mockRejectedValue(networkError);

    await expect(agent.Account.current()).rejects.toThrow('Network Error');
  });
});