// src/services/doctorService.js

import api from './api';

// ============================================================
// HELPERS
// ============================================================

/**
 * Hỗ trợ cả 2 kiểu api.js:
 *
 * 1. Axios mặc định:
 *    response = { data: ... }
 *
 * 2. api.js đã có interceptor:
 *    response = dữ liệu thật
 */
const unwrap = (response) => {
  if (response == null) {
    return response;
  }

  if (
    typeof response === 'object' &&
    Object.prototype.hasOwnProperty.call(response, 'data')
  ) {
    return response.data;
  }

  return response;
};

/**
 * Loại bỏ query parameter null / undefined / chuỗi rỗng.
 */
const cleanParams = (params = {}) =>
  Object.fromEntries(
    Object.entries(params).filter(
      ([, value]) =>
        value !== undefined &&
        value !== null &&
        value !== ''
    )
  );

const encodeId = (value) =>
  encodeURIComponent(String(value ?? '').trim());

const requireId = (value, fieldName = 'id') => {
  const id = String(value ?? '').trim();

  if (!id) {
    throw new Error(`Thiếu ${fieldName}.`);
  }

  return id;
};

// ============================================================
// DOCTOR / PUBLIC INFORMATION
// ============================================================

/**
 * Danh sách bác sĩ.
 *
 * Có thể truyền:
 * {
 *   specialtyId,
 *   facilityId,
 *   q
 * }
 */
export const getDoctors = async (params = {}) => {
  const response = await api.get('/doctors', {
    params: cleanParams(params),
  });

  return unwrap(response);
};

/**
 * Chi tiết bác sĩ.
 */
export const getDoctorById = async (doctorId) => {
  requireId(doctorId, 'mã bác sĩ');

  const response = await api.get(
    `/doctors/${encodeId(doctorId)}`
  );

  return unwrap(response);
};

// ============================================================
// DOCTOR SCHEDULE
// ============================================================

/**
 * Lịch làm việc của một bác sĩ.
 *
 * date:
 * yyyy-MM-dd
 */
export const getDoctorSchedule = async (
  doctorId,
  date = null
) => {
  requireId(doctorId, 'mã bác sĩ');

  const response = await api.get(
    `/schedules/doctors/${encodeId(doctorId)}`,
    {
      params: cleanParams({
        date,
      }),
    }
  );

  return unwrap(response);
};

// ============================================================
// DOCTOR QUEUE / VISITS
// ============================================================

/**
 * Danh sách bệnh nhân đang chờ bác sĩ.
 *
 * Backend:
 * GET /api/doctor/visits/waiting
 */
export const getDoctorWaitingVisits = async () => {
  const response = await api.get(
    '/doctor/visits/waiting'
  );

  const data = unwrap(response);

  if (Array.isArray(data)) {
    return data;
  }

  return (
    data?.items ??
    data?.content ??
    data?.data ??
    []
  );
};

/**
 * Alias ngắn.
 */
export const getDoctorQueue =
  getDoctorWaitingVisits;

/**
 * Alias tương thích frontend cũ.
 */
export const getWaitingVisits =
  getDoctorWaitingVisits;

export const getWaitingQueue =
  getDoctorWaitingVisits;

/**
 * Bác sĩ gọi bệnh nhân vào phòng.
 *
 * POST /api/doctor/visits/{id}/call
 */
export const callDoctorVisit = async (visitId) => {
  requireId(visitId, 'mã lượt khám');

  const response = await api.post(
    `/doctor/visits/${encodeId(visitId)}/call`
  );

  return unwrap(response);
};

export const callVisit =
  callDoctorVisit;

export const callPatient =
  callDoctorVisit;

/**
 * Bệnh nhân vắng / tạm bỏ qua.
 *
 * POST /api/doctor/visits/{id}/skip
 */
export const skipDoctorVisit = async (visitId) => {
  requireId(visitId, 'mã lượt khám');

  const response = await api.post(
    `/doctor/visits/${encodeId(visitId)}/skip`
  );

  return unwrap(response);
};

export const skipVisit =
  skipDoctorVisit;

export const skipPatient =
  skipDoctorVisit;

/**
 * Bắt đầu khám.
 *
 * Backend sẽ:
 * - kiểm tra bác sĩ đang đăng nhập
 * - chuyển luotkham -> dang_kham
 * - set ThoiGianBatDau
 * - tạo bản ghi kham nếu chưa có
 */
export const startDoctorVisit = async (visitId) => {
  requireId(visitId, 'mã lượt khám');

  const response = await api.post(
    `/doctor/visits/${encodeId(visitId)}/start`
  );

  return unwrap(response);
};

export const startVisit =
  startDoctorVisit;

export const startExam =
  startDoctorVisit;

// ============================================================
// EXAMINATION
// ============================================================

/**
 * Lấy thông tin khám theo lượt khám.
 *
 * Hàm này giữ lại để tương thích các màn KhamBenh cũ.
 */
export const getExamination = async (visitId) => {
  requireId(visitId, 'mã lượt khám');

  const response = await api.get(
    `/doctor/examinations/${encodeId(visitId)}`
  );

  return unwrap(response);
};

export const getExaminationByVisit =
  getExamination;

/**
 * Lưu/cập nhật thông tin khám.
 *
 * payload ví dụ:
 * {
 *   trieuChung,
 *   chanDoan,
 *   ketLuan,
 *   ghiChu
 * }
 */
export const saveExamination = async (
  visitId,
  payload = {}
) => {
  requireId(visitId, 'mã lượt khám');

  const response = await api.put(
    `/doctor/examinations/${encodeId(visitId)}`,
    payload
  );

  return unwrap(response);
};

export const updateExamination =
  saveExamination;

/**
 * Hoàn tất khám nếu ExaminationController đang dùng
 * action /complete.
 */
export const completeExamination = async (
  visitId,
  payload = {}
) => {
  requireId(visitId, 'mã lượt khám');

  const response = await api.post(
    `/doctor/examinations/${encodeId(
      visitId
    )}/complete`,
    payload
  );

  return unwrap(response);
};

// ============================================================
// TEST CATALOG FOR DOCTOR
// ============================================================

/**
 * Danh sách loại xét nghiệm phục vụ màn chỉ định.
 */
export const getDoctorTests = async (
  params = {}
) => {
  const response = await api.get('/tests', {
    params: cleanParams(params),
  });

  const data = unwrap(response);

  if (Array.isArray(data)) {
    return data;
  }

  return (
    data?.items ??
    data?.content ??
    data?.data ??
    []
  );
};

export const getTests =
  getDoctorTests;

// ============================================================
// TEST ORDER
// ============================================================

/**
 * Tạo phiếu/chỉ định xét nghiệm từ lượt khám.
 *
 * payload thường gồm:
 * {
 *   visitId,
 *   testIds,
 *   note
 * }
 */
export const createTestOrder = async (
  payload = {}
) => {
  const response = await api.post(
    '/doctor/test-orders',
    payload
  );

  return unwrap(response);
};

export const orderTests =
  createTestOrder;

/**
 * Lấy phiếu xét nghiệm theo lượt khám.
 */
export const getTestOrdersByVisit = async (
  visitId
) => {
  requireId(visitId, 'mã lượt khám');

  const response = await api.get(
    '/doctor/test-orders',
    {
      params: {
        visitId,
      },
    }
  );

  const data = unwrap(response);

  return Array.isArray(data)
    ? data
    : data?.items ??
        data?.content ??
        data?.data ??
        [];
};

// ============================================================
// TECHNICIANS
// ============================================================

/**
 * Danh sách KTV đang hoạt động để bác sĩ bàn giao mẫu.
 */
export const getTechnicians = async () => {
  const response = await api.get(
    '/doctor/technicians'
  );

  const data = unwrap(response);

  if (Array.isArray(data)) {
    return data;
  }

  return (
    data?.items ??
    data?.content ??
    data?.data ??
    []
  );
};

export const getActiveTechnicians =
  getTechnicians;

// ============================================================
// SPECIMEN - DOCTOR
// ============================================================

/**
 * Bác sĩ xác nhận lấy mẫu.
 *
 * Backend:
 * POST /api/doctor/specimens
 *
 * payload:
 * {
 *   appointmentId,
 *   patientCode,
 *   sampleType,
 *   barcode,
 *   notes
 * }
 */
export const collectSpecimen = async (
  payload = {}
) => {
  const response = await api.post(
    '/doctor/specimens',
    payload
  );

  return unwrap(response);
};

/**
 * Alias để tương thích tên cũ.
 */
export const createSpecimen =
  collectSpecimen;

/**
 * Bàn giao mẫu cho KTV.
 *
 * specimenId có thể là mã mẫu / barcode tùy DTO backend
 * đang sử dụng.
 *
 * payload:
 * {
 *   appointmentId,
 *   patientCode,
 *   receiverId,
 *   notes
 * }
 */
export const handoverSpecimen = async (
  specimenId,
  payload = {}
) => {
  requireId(
    specimenId,
    'mã mẫu'
  );

  const response = await api.post(
    `/doctor/specimens/${encodeId(
      specimenId
    )}/handover`,
    payload
  );

  return unwrap(response);
};

/**
 * Alias tương thích code cũ.
 */
export const handOverSpecimen =
  handoverSpecimen;

// ============================================================
// TEST RESULT - DOCTOR
// ============================================================

/**
 * Danh sách kết quả đang chờ bác sĩ duyệt.
 *
 * Backend Module 11:
 * GET /api/doctor/results/pending
 */
export const getPendingDoctorResults =
  async () => {
    const response = await api.get(
      '/doctor/results/pending'
    );

    const data = unwrap(response);

    if (Array.isArray(data)) {
      return data;
    }

    return (
      data?.items ??
      data?.content ??
      data?.data ??
      []
    );
  };

/**
 * Alias cho code cũ.
 */
export const getPendingResults =
  getPendingDoctorResults;

/**
 * Chi tiết một kết quả xét nghiệm.
 *
 * Backend:
 * GET /api/doctor/results/{resultId}
 */
export const getDoctorResultDetail =
  async (resultId) => {
    requireId(
      resultId,
      'mã kết quả'
    );

    const response = await api.get(
      `/doctor/results/${encodeId(
        resultId
      )}`
    );

    return unwrap(response);
  };

export const getResultDetail =
  getDoctorResultDetail;

/**
 * Bác sĩ duyệt kết quả.
 *
 * Backend:
 * POST /api/doctor/results/{resultId}/approve
 *
 * Có thể gọi:
 *
 * approveDoctorResult(id, 'Kết luận...')
 *
 * hoặc:
 *
 * approveDoctorResult(id, {
 *   conclusion: 'Kết luận...'
 * })
 */
export const approveDoctorResult = async (
  resultId,
  conclusion
) => {
  requireId(
    resultId,
    'mã kết quả'
  );

  let payload;

  if (
    conclusion &&
    typeof conclusion === 'object' &&
    !Array.isArray(conclusion)
  ) {
    payload = {
      ...conclusion,
      conclusion:
        conclusion.conclusion ??
        '',
    };
  } else {
    payload = {
      conclusion:
        String(
          conclusion ??
          ''
        ).trim(),
    };
  }

  const response = await api.post(
    `/doctor/results/${encodeId(
      resultId
    )}/approve`,
    payload
  );

  return unwrap(response);
};

export const approveResult =
  approveDoctorResult;

// ============================================================
// GENERAL RESULT HELPERS
// ============================================================

/**
 * Kiểm tra một trạng thái kết quả có phải đang chờ duyệt hay không.
 */
export const isPendingApproval = (status) => {
  const value = String(
    status ?? ''
  )
    .trim()
    .toUpperCase();

  return (
    value === 'PENDING_APPROVAL' ||
    value === 'CHO_DUYET'
  );
};

/**
 * Chuẩn hóa trạng thái kết quả từ DB/API.
 */
export const normalizeResultStatus = (
  status
) => {
  const value = String(
    status ?? ''
  )
    .trim()
    .toLowerCase();

  switch (value) {
    case 'dang_thuc_hien':
    case 'in_progress':
      return 'IN_PROGRESS';

    case 'cho_duyet':
    case 'pending_approval':
      return 'PENDING_APPROVAL';

    case 'da_duyet':
    case 'approved':
      return 'APPROVED';

    case 'can_lam_lai':
    case 'needs_rerun':
      return 'NEEDS_RERUN';

    case 'hoan_tat':
    case 'completed':
      return 'COMPLETED';

    default:
      return String(
        status ??
        ''
      ).toUpperCase();
  }
};

// ============================================================
// DEFAULT EXPORT
// ============================================================

/**
 * Giữ default export để các file cũ có thể dùng:
 *
 * import doctorService from '../../services/doctorService';
 *
 * đồng thời vẫn hỗ trợ:
 *
 * import {
 *   getPendingDoctorResults
 * } from '../../services/doctorService';
 */
const doctorService = {
  // Doctor
  getDoctors,
  getDoctorById,
  getDoctorSchedule,

  // Queue / visit
  getDoctorWaitingVisits,
  getDoctorQueue,
  getWaitingVisits,
  getWaitingQueue,

  callDoctorVisit,
  callVisit,
  callPatient,

  skipDoctorVisit,
  skipVisit,
  skipPatient,

  startDoctorVisit,
  startVisit,
  startExam,

  // Examination
  getExamination,
  getExaminationByVisit,
  saveExamination,
  updateExamination,
  completeExamination,

  // Test
  getDoctorTests,
  getTests,

  // Test order
  createTestOrder,
  orderTests,
  getTestOrdersByVisit,

  // Technician
  getTechnicians,
  getActiveTechnicians,

  // Specimen
  collectSpecimen,
  createSpecimen,
  handoverSpecimen,
  handOverSpecimen,

  // Result
  getPendingDoctorResults,
  getPendingResults,
  getDoctorResultDetail,
  getResultDetail,
  approveDoctorResult,
  approveResult,

  // Helpers
  isPendingApproval,
  normalizeResultStatus,
};

export default doctorService;