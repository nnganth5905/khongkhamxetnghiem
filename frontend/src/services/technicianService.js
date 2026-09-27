import api, {
  unwrap,
} from './api';

// =====================================================
// CURRENT TECHNICIAN
// =====================================================

export const getCurrentTechnician =
  async () =>
    unwrap(
      await api.get(
        '/technician/me',
      ),
    );

// =====================================================
// SPECIMEN
// =====================================================

export const getSpecimens =
  async (
    params = {},
  ) =>
    unwrap(
      await api.get(
        '/technician/specimens',
        {
          params,
        },
      ),
    );

export const getSpecimen =
  async (
    id,
  ) =>
    unwrap(
      await api.get(
        `/technician/specimens/${encodeURIComponent(
          id,
        )}`,
      ),
    );

export const getSpecimenById =
  getSpecimen;

export const receiveSpecimen =
  async (
    id,
    data = {},
  ) =>
    unwrap(
      await api.post(
        `/technician/specimens/${encodeURIComponent(
          id,
        )}/receive`,
        data,
      ),
    );

export const rejectSpecimen =
  async (
    id,
    data = {},
  ) =>
    unwrap(
      await api.post(
        `/technician/specimens/${encodeURIComponent(
          id,
        )}/reject`,
        data,
      ),
    );

// =====================================================
// WORKLIST
// =====================================================

export const getWorklist =
  async (
    params = {},
  ) =>
    unwrap(
      await api.get(
        '/technician/worklist',
        {
          params,
        },
      ),
    );

export const startWork =
  async (
    id,
    data = {},
  ) =>
    unwrap(
      await api.post(
        `/technician/worklist/${encodeURIComponent(
          id,
        )}/start`,
        data,
      ),
    );

export const completeWork =
  async (
    id,
    data = {},
  ) =>
    unwrap(
      await api.post(
        `/technician/worklist/${encodeURIComponent(
          id,
        )}/complete`,
        data,
      ),
    );

// =====================================================
// RESULT ENTRY
// =====================================================

export const getResultEntry =
  async (
    id,
  ) =>
    unwrap(
      await api.get(
        `/technician/results/${encodeURIComponent(
          id,
        )}`,
      ),
    );

export const saveResultEntry =
  async (
    id,
    data,
  ) =>
    unwrap(
      await api.put(
        `/technician/results/${encodeURIComponent(
          id,
        )}`,
        data,
      ),
    );

export const submitResultEntry =
  async (
    id,
    data,
  ) =>
    unwrap(
      await api.post(
        `/technician/results/${encodeURIComponent(
          id,
        )}/submit`,
        data,
      ),
    );

export const submitTestResult =
  async (
    specimenId,
    resultData,
  ) =>
    unwrap(
      await api.post(
        `/technician/specimens/${encodeURIComponent(
          specimenId,
        )}/results`,
        resultData,
      ),
    );