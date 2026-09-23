import api from './api';

const visitService = {
  // Lấy danh sách hàng chờ cho Lễ tân
  getReceptionWaitingList: async (type = 'ALL') => {
    try {
      const response = await api.get('/reception/visits/waiting', {
        params: { type }
      });
      return response.data;
    } catch (error) {
      console.error("Lỗi khi lấy danh sách chờ:", error);
      throw error;
    }
  }
};

export default visitService;