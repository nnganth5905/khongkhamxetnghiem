import api, { unwrap } from "./api";

/**
 * Lấy toàn bộ danh sách xét nghiệm
 * GET /api/tests
 */
export async function getTests(params = {}) {
  const response = await api.get("/tests", {
    params,
  });

  return unwrap(response);
}

/**
 * Lấy chi tiết một xét nghiệm
 * GET /api/tests/{id}
 */
export async function getTestById(id) {
  const response = await api.get(
    `/tests/${id}`,
  );

  return unwrap(response);
}

/**
 * Lấy xét nghiệm theo danh mục
 * GET /api/tests/category/{slug}
 */
export async function getTestsByCategory(
  slug,
  params = {},
) {
  const response = await api.get(
    `/tests/category/${encodeURIComponent(slug)}`,
    {
      params,
    },
  );

  return unwrap(response);
}


/*
 * =====================================================
 * ALIAS
 * Giữ tương thích với các page đã viết trước đó
 * =====================================================
 */

export const getTest =
  getTestById;

export const getCategoryTests =
  getTestsByCategory;


/*
 * =====================================================
 * DEFAULT EXPORT
 * =====================================================
 */

const testService = {
  getTests,
  getTestById,
  getTestsByCategory,

  getTest,
  getCategoryTests,
};

export default testService;