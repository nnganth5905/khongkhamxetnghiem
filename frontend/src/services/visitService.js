import api from './api';

const visitService = {
  getReceptionWaitingList: async (type = 'ALL') => {
    const response = await api.get('/reception/visits/waiting', { params: { type } });
    return response.data;
  },
  
  // Các API dành cho Bác sĩ
  getDoctorQueue: async () => {
    const response = await api.get('/doctor/queue');
    return response.data;
  },

  callPatient: async (id) => {
    const response = await api.post(`/doctor/visits/${id}/call`);
    return response.data;
  },

  skipPatient: async (id) => {
    const response = await api.post(`/doctor/visits/${id}/skip`);
    return response.data;
  },

  startExam: async (id) => {
    const response = await api.post(`/doctor/visits/${id}/start`);
    return response.data;
  },
// Lấy dữ liệu khám
  getExamination: async (luotKhamId) => {
    const response = await api.get(`/doctor/examinations/visit/${luotKhamId}`);
    return response.data;
  },

  // Lưu hồ sơ bệnh án (Gọi PUT /api/doctor/examinations/{id})
  saveExamination: async (idKham, data, luotKhamId) => {
    // Map dữ liệu từ state của Frontend vào ExaminationRequest của Backend
    const payload = {
        visitId: luotKhamId,
        symptoms: data.trieuChung,
        history: data.tienSuBenh,
        diagnosis: data.chanDoan,
        conclusion: data.ketLuan,
        advice: data.huongDieuTri
    };
    const response = await api.put(`/doctor/examinations/${idKham}`, payload);
    return response.data;
  },

  // Hoàn tất khám (Gọi POST /api/doctor/examinations/{id}/complete)
  completeExamination: async (luotKhamId) => {
    const response = await api.post(`/doctor/examinations/${luotKhamId}/complete`);
    return response.data;
  }

};

export default visitService;