import type { Vehicle, CreateVehicleRequest, VehicleFilters, PagedResult } from '~/types/vehicle';

export const useVehicleApi = () => {
  const config = useRuntimeConfig();
  const apiBase = config.public.apiBase;

  const getVehicles = async (filters?: VehicleFilters): Promise<PagedResult<Vehicle>> => {
    const params = new URLSearchParams();
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters?.brand) params.append('brand', filters.brand);
    if (filters?.status) params.append('status', filters.status);

    const response = await $etch<PagedResult<Vehicle>>(${apiBase}/v1/vehicles?${params.toString()});
    return response;
  };

  const createVehicle = async (data: CreateVehicleRequest): Promise<{ id: string }> => {
    const response = await $etch<{ id: string }>(${apiBase}/v1/vehicles, {
      method: 'POST',
      body: data
    });
    return response;
  };

  return {
    getVehicles,
    createVehicle
  };
};
