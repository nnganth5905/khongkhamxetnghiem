// src/services/trackingService.js

import api from './api';

// ============================================================
// HELPERS
// ============================================================

const unwrap = (response) => {
  if (response == null) {
    return response;
  }

  if (
    typeof response === 'object' &&
    Object.prototype.hasOwnProperty.call(
      response,
      'data'
    )
  ) {
    return response.data;
  }

  return response;
};

const encodeId = (value) =>
  encodeURIComponent(
    String(value ?? '').trim()
  );

const requireId = (
  value,
  fieldName = 'id'
) => {
  const id = String(
    value ?? ''
  ).trim();

  if (!id) {
    throw new Error(
      `Thiếu ${fieldName}.`
    );
  }

  return id;
};

const cleanParams = (
  params = {}
) =>
  Object.fromEntries(
    Object.entries(
      params
    ).filter(
      ([, value]) =>
        value !== undefined &&
        value !== null &&
        value !== ''
    )
  );

// ============================================================
// VISIT TRACKING
// ============================================================

/**
 * Theo dõi 1 lượt khám.
 *
 * id:
 * - IDLuotKham
 * - hoặc MaDatLich DLK...
 */
export const getVisitTracking =
  async (id) => {
    requireId(
      id,
      'mã lượt khám'
    );

    const response =
      await api.get(
        `/tracking/visits/${encodeId(
          id
        )}`
      );

    return unwrap(response);
  };

/**
 * Alias cũ.
 */
export const getExaminationTracking =
  getVisitTracking;

export const getVisitTimeline =
  getVisitTracking;

// ============================================================
// TEST TRACKING
// ============================================================

/**
 * Theo dõi 1 lượt xét nghiệm.
 *
 * id:
 * - IDLuotXetNghiem
 * - hoặc MaDatLich DLXN...
 */
export const getTestTracking =
  async (id) => {
    requireId(
      id,
      'mã lượt xét nghiệm'
    );

    const response =
      await api.get(
        `/tracking/tests/${encodeId(
          id
        )}`
      );

    return unwrap(response);
  };

export const getTestTimeline =
  getTestTracking;

export const getLaboratoryTracking =
  getTestTracking;

// ============================================================
// CUSTOMER HISTORY
// ============================================================

/**
 * Lịch sử truy vết.
 *
 * CUSTOMER:
 *   không cần truyền customerId.
 *
 * ADMIN / DOCTOR / ...:
 *   có thể truyền customerId.
 *
 * options:
 * {
 *   customerId,
 *   type,
 *   limit
 * }
 */
export const getTrackingHistory =
  async (options = {}) => {
    const response =
      await api.get(
        '/tracking/history',
        {
          params: cleanParams(
            options
          ),
        }
      );

    const data =
      unwrap(response);

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

export const getHistory =
  getTrackingHistory;

export const getAuditHistory =
  getTrackingHistory;

// ============================================================
// OBJECT HISTORY
// ============================================================

/**
 * Lịch sử của một đối tượng.
 *
 * Ví dụ:
 *
 * getObjectHistory(
 *   'luotkham',
 *   12
 * )
 */
export const getObjectHistory =
  async (
    objectType,
    objectId
  ) => {
    requireId(
      objectType,
      'loại đối tượng'
    );

    requireId(
      objectId,
      'mã đối tượng'
    );

    const response =
      await api.get(
        `/tracking/object/${encodeId(
          objectType
        )}/${encodeId(
          objectId
        )}`
      );

    const data =
      unwrap(response);

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

// ============================================================
// MY VISITS
// ============================================================

/**
 * Danh sách lượt khám
 * của khách hàng đang đăng nhập.
 */
export const getMyVisits =
  async () => {
    const response =
      await api.get(
        '/tracking/my-visits'
      );

    const data =
      unwrap(response);

    return Array.isArray(data)
      ? data
      : data?.items ??
          data?.content ??
          data?.data ??
          [];
  };

// ============================================================
// MY TEST VISITS
// ============================================================

/**
 * Danh sách lượt xét nghiệm
 * của khách hàng đang đăng nhập.
 */
export const getMyTestVisits =
  async () => {
    const response =
      await api.get(
        '/tracking/my-tests'
      );

    const data =
      unwrap(response);

    return Array.isArray(data)
      ? data
      : data?.items ??
          data?.content ??
          data?.data ??
          [];
  };

export const getMyTests =
  getMyTestVisits;

// ============================================================
// NORMALIZE TIMELINE
// ============================================================

export const normalizeTimeline =
  (timeline = []) => {
    if (!Array.isArray(timeline)) {
      return [];
    }

    return timeline
      .map((item, index) => ({
        id:
          item.id ??
          `${item.time ?? index}-${index}`,

        time:
          item.time ??
          item.thoiGian ??
          item.ThoiGian ??
          null,

        code:
          item.code ??
          item.eventCode ??
          '',

        event:
          item.event ??
          item.action ??
          item.hanhDong ??
          'Cập nhật',

        description:
          item.description ??
          item.moTa ??
          item.MoTa ??
          '',

        oldStatus:
          item.oldStatus ??
          item.trangThaiCu ??
          '',

        newStatus:
          item.newStatus ??
          item.trangThaiMoi ??
          '',

        source:
          item.source ??
          item.nguonThucHien ??
          '',

        userId:
          item.userId ??
          item.performedByUserId ??
          null,
      }))
      .sort((a, b) => {
        const timeA =
          a.time
            ? new Date(
                a.time
              ).getTime()
            : 0;

        const timeB =
          b.time
            ? new Date(
                b.time
              ).getTime()
            : 0;

        return timeA - timeB;
      });
  };

// ============================================================
// STATUS
// ============================================================

export const normalizeTrackingStatus =
  (status) => {
    const value =
      String(
        status ?? ''
      )
        .trim()
        .toLowerCase();

    switch (value) {
      // =========================================
      // LƯỢT KHÁM
      // =========================================

      case 'da_tiep_nhan':
        return 'CHECKED_IN';

      case 'cho_kham':
        return 'WAITING';

      case 'da_den_luot':
        return 'CALLED';

      case 'cho_goi_lai':
        return 'RECALL';

      case 'dang_kham':
        return 'EXAMINING';

      case 'da_chi_dinh_xn':
        return 'TEST_ORDERED';

      case 'moi_doc_kq':
        return 'RESULT_READY';

      case 'dang_tu_van_kq':
        return 'CONSULTING';

      case 'bo_luot':
        return 'SKIPPED';

      // =========================================
      // XÉT NGHIỆM
      // =========================================

      case 'cho_xet_nghiem':
        return 'WAITING_TEST';

      case 'dang_lay_mau':
        return 'COLLECTING';

      case 'da_lay_mau':
        return 'COLLECTED';

      case 'ktv_tiep_nhan':
        return 'RECEIVED';

      case 'dang_xet_nghiem':
        return 'IN_PROGRESS';

      case 'cho_duyet_kq':
      case 'cho_duyet':
        return 'PENDING_APPROVAL';

      case 'da_co_kq':
        return 'RESULT_READY';

      // =========================================
      // CHUNG
      // =========================================

      case 'hoan_tat':
      case 'completed':
        return 'COMPLETED';

      case 'huy':
      case 'cancelled':
        return 'CANCELLED';

      case 'pending':
        return 'PENDING';

      case 'confirmed':
        return 'CONFIRMED';

      case 'checked_in':
        return 'CHECKED_IN';

      default:
        return String(
          status ?? ''
        ).toUpperCase();
    }
  };

// ============================================================
// STATUS LABEL
// ============================================================

export const getTrackingStatusLabel =
  (status) => {
    const value =
      normalizeTrackingStatus(
        status
      );

    const labels = {
      PENDING:
        'Chờ xác nhận',

      CONFIRMED:
        'Đã xác nhận',

      CHECKED_IN:
        'Đã tiếp nhận',

      WAITING:
        'Đang chờ khám',

      CALLED:
        'Đã đến lượt',

      RECALL:
        'Chờ gọi lại',

      EXAMINING:
        'Đang khám',

      TEST_ORDERED:
        'Đã chỉ định xét nghiệm',

      WAITING_TEST:
        'Chờ xét nghiệm',

      COLLECTING:
        'Đang lấy mẫu',

      COLLECTED:
        'Đã lấy mẫu',

      RECEIVED:
        'KTV đã tiếp nhận',

      IN_PROGRESS:
        'Đang xét nghiệm',

      PENDING_APPROVAL:
        'Chờ bác sĩ duyệt',

      RESULT_READY:
        'Đã có kết quả',

      CONSULTING:
        'Đang tư vấn kết quả',

      SKIPPED:
        'Bỏ lượt',

      COMPLETED:
        'Hoàn tất',

      CANCELLED:
        'Đã hủy',
    };

    return (
      labels[value] ??
      status ??
      'Không xác định'
    );
  };

// ============================================================
// EVENT LABEL
// ============================================================

export const getTrackingEventLabel =
  (code, fallback = '') => {
    const value =
      String(
        code ?? ''
      ).toUpperCase();

    const labels = {
      BOOKED:
        'Đặt lịch',

      APPOINTMENT_RESCHEDULED:
        'Thay đổi lịch hẹn',

      APPOINTMENT_CANCELLED:
        'Hủy lịch hẹn',

      CHECKED_IN:
        'Check-in',

      CALLED:
        'Được gọi vào phòng',

      SKIPPED:
        'Chờ gọi lại',

      EXAM_STARTED:
        'Bắt đầu khám',

      EXAM_COMPLETED:
        'Hoàn tất khám',

      STARTED:
        'Bắt đầu quy trình',

      SPECIMEN_COLLECTED:
        'Đã lấy mẫu',

      SPECIMEN_HANDED_OVER:
        'Đã bàn giao mẫu',

      SPECIMEN_RECEIVED:
        'KTV tiếp nhận mẫu',

      WORKLIST_RECEIVED:
        'Đã vào worklist',

      TEST_IN_PROGRESS:
        'Đang xét nghiệm',

      TEST_FINISHED:
        'Hoàn tất xét nghiệm',

      RESULT_ENTERED:
        'Đã nhập kết quả',

      RESULT_SUBMITTED:
        'Đã gửi duyệt',

      RESULT_APPROVED:
        'Đã duyệt kết quả',

      RESULT_AVAILABLE:
        'Đã có kết quả',

      COMPLETED:
        'Hoàn tất quy trình',
    };

    return (
      labels[value] ||
      fallback ||
      code ||
      'Cập nhật'
    );
  };

// ============================================================
// DEFAULT EXPORT
// ============================================================

const trackingService = {
  getVisitTracking,
  getExaminationTracking,
  getVisitTimeline,

  getTestTracking,
  getTestTimeline,
  getLaboratoryTracking,

  getTrackingHistory,
  getHistory,
  getAuditHistory,

  getObjectHistory,

  getMyVisits,
  getMyTestVisits,
  getMyTests,

  normalizeTimeline,
  normalizeTrackingStatus,
  getTrackingStatusLabel,
  getTrackingEventLabel,
};

export default trackingService;