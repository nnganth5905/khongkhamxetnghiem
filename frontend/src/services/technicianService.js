// src/services/technicianService.js
import api, { unwrap } from './api';

// --- QUẢN LÝ MẪU BỆNH PHẨM ---
export const getSpecimens = async (params = {}) => {
  const response = await api.get('/technician/specimens', { params });
  return unwrap(response);
};

export const getSpecimen = async (id) => {
  const response = await api.get(`/technician/specimens/${id}`);
  return unwrap(response);
};
export const getSpecimenById = getSpecimen;

export const receiveSpecimen = async (id, data = {}) => {
  const response = await api.post(`/technician/specimens/${id}/receive`, data);
  return unwrap(response);
};

export const rejectSpecimen = async (id, data = {}) => {
  const response = await api.post(`/technician/specimens/${id}/reject`, data);
  return unwrap(response);
};

export const handoverSpecimen = async (id, data = {}) => {
  const response = await api.post(`/technician/specimens/${id}/handover`, data);
  return unwrap(response);
};

export const updateSpecimenStatus = async (id, data) => {
  const response = await api.put(`/technician/specimens/${id}`, data);
  return unwrap(response);
};

// --- QUY TRÌNH THỰC HIỆN XÉT NGHIỆM & WORKLIST ---
export const getWorklist = async (params = {}) => {
  const response = await api.get('/technician/worklist', { params });
  return unwrap(response);
};

export const getWorkDetail = async (id) => {
  const response = await api.get(`/technician/worklist/${id}`);
  return unwrap(response);
};

export const startWork = async (id, data = {}) => {
  const response = await api.post(`/technician/worklist/${id}/start`, data);
  return unwrap(response);
};

export const completeWork = async (id, data = {}) => {
  const response = await api.post(`/technician/worklist/${id}/complete`, data);
  return unwrap(response);
};

// --- NHẬP VÀ XỬ LÝ KẾT QUẢ ---
export const getResultEntry = async (id) => {
  const response = await api.get(`/technician/results/${id}`);
  return unwrap(response);
};

export const saveResultEntry = async (id, data) => {
  const response = await api.put(`/technician/results/${id}`, data);
  return unwrap(response);
};

export const submitResultEntry = async (id, data) => {
  const response = await api.post(`/technician/results/${id}/submit`, data);
  return unwrap(response);
};

export const submitTestResult = async (id, resultData) => {
  const response = await api.post(`/technician/specimens/${id}/results`, resultData);
  return unwrap(response);
};