import React, {
  useEffect,
  useState,
} from 'react';

import {
  getHandbook,
  getMedicalErrorMessage,
} from '../../services/medicalService';

const DEFAULT_CATEGORIES = [
  {
    id: 1,
    name:
      'Bệnh lý Thận - Tiết niệu',

    tests: [
      {
        id: 1,
        name:
          'Tổng phân tích nước tiểu (10 thông số)',
      },
      {
        id: 2,
        name:
          'Định lượng Ure, Creatinine máu',
      },
      {
        id: 3,
        name:
          'Định lượng Microalbumin niệu',
      },
    ],
  },
  {
    id: 2,
    name:
      'Bệnh lý Tim mạch & Chuyển hóa',

    tests: [
      {
        id: 4,
        name:
          'Bộ mỡ máu (Cholesterol, Triglycerid, HDL-C, LDL-C)',
      },
      {
        id: 5,
        name:
          'Định lượng Glucose máu, HbA1c',
      },
      {
        id: 6,
        name:
          'Định lượng Troponin T/I, CK-MB',
      },
    ],
  },
  {
    id: 3,
    name:
      'Bệnh lý Gan mật',

    tests: [
      {
        id: 7,
        name:
          'Men gan (AST, ALT, GGT)',
      },
      {
        id: 8,
        name:
          'Định lượng Bilirubin toàn phần, trực tiếp, gián tiếp',
      },
      {
        id: 9,
        name:
          'Sàng lọc Viêm gan B, C (HBsAg, Anti-HCV)',
      },
    ],
  },
];

const normalizeCategories = (
  data
) => {
  const source =
    Array.isArray(data)
      ? data
      : data?.categories ??
        data?.content ??
        [];

  return source.map(
    (category) => ({
      id:
        category.id ??
        category.idDanhMuc ??
        category.IDDanhMuc,

      name:
        category.name ??
        category.tenDanhMuc ??
        category.TenDanhMuc ??
        '',

      tests:
        (
          category.tests ??
          category.xetNghiems ??
          category.danhSachXetNghiem ??
          []
        ).map((test) => ({
          id:
            test.id ??
            test.idXetNghiem ??
            test.IDXetNghiem,

          name:
            test.name ??
            test.tenXetNghiem ??
            test.TenXetNghiem ??
            '',
        })),
    })
  );
};

export default function Handbook({
  initialCategories = null,
}) {
  const [
    categories,
    setCategories,
  ] = useState(
    initialCategories ||
      DEFAULT_CATEGORIES
  );

  const [loading, setLoading] =
    useState(
      !initialCategories
    );

  const [error, setError] =
    useState('');

  useEffect(() => {
    if (
      initialCategories
    ) {
      return;
    }

    let active = true;

    const loadHandbook =
      async () => {
        try {
          setLoading(true);
          setError('');

          const data =
            await getHandbook();

          if (!active) {
            return;
          }

          const result =
            normalizeCategories(
              data
            );

          if (
            result.length > 0
          ) {
            setCategories(
              result
            );
          }
        } catch (err) {
          if (!active) {
            return;
          }

          /*
           * Vẫn giữ fallback khi API chưa có.
           */
          setError(
            getMedicalErrorMessage(
              err,
              ''
            )
          );
        } finally {
          if (active) {
            setLoading(false);
          }
        }
      };

    loadHandbook();

    return () => {
      active = false;
    };
  }, [
    initialCategories,
  ]);

  return (
    <div
      className="container-custom py-4"
      style={{
        maxWidth:
          '1200px',

        margin:
          '0 auto',

        padding:
          '0 15px',
      }}
    >
      <div
        className="section-title text-start fw-bold mb-4"
        style={{
          fontSize:
            '28px',

          textTransform:
            'uppercase',

          color:
            'var(--primary, #0b63e5)',

          borderBottom:
            '3px solid var(--primary, #0b63e5)',

          display:
            'inline-block',

          paddingBottom:
            '4px',
        }}
      >
        Cẩm nang bệnh học
      </div>

      {error && (
        <div className="alert alert-warning small">
          Không thể tải dữ
          liệu mới từ máy chủ.
          Đang hiển thị dữ liệu
          mặc định.
        </div>
      )}

      <div className="d-flex flex-wrap gap-4 align-items-start">
        <main
          className="center-col flex-grow-1"
          style={{
            minWidth:
              '300px',
          }}
        >
          {loading ? (
            <div className="text-secondary py-4 text-center">
              Đang tải cẩm
              nang...
            </div>
          ) : categories &&
            categories.length >
              0 ? (
            categories.map(
              (cat) => (
                <div
                  key={
                    cat.id
                  }
                  className="category-card mb-4 p-4 rounded-3 shadow-sm border-0"
                  style={{
                    background:
                      '#f9f9f9',
                  }}
                >
                  <div
                    className="category-title fw-bold mb-3"
                    style={{
                      fontSize:
                        '20px',

                      color:
                        'var(--primary, #0b63e5)',

                      borderLeft:
                        '5px solid var(--primary, #0b63e5)',

                      paddingLeft:
                        '10px',
                    }}
                  >
                    {cat.name}
                  </div>

                  <div>
                    {cat.tests &&
                    cat.tests
                      .length >
                      0 ? (
                      cat.tests.map(
                        (
                          test
                        ) => (
                          <div
                            key={
                              test.id
                            }
                            className="test-item py-2 border-bottom text-secondary"
                            style={{
                              fontSize:
                                '15px',
                            }}
                          >
                            <span
                              style={{
                                color:
                                  'var(--primary, #0b63e5)',

                                fontWeight:
                                  500,

                                marginRight:
                                  '6px',
                              }}
                            >
                              •
                            </span>

                            {
                              test.name
                            }
                          </div>
                        )
                      )
                    ) : (
                      <div className="text-secondary small">
                        Chưa có xét
                        nghiệm trong
                        danh mục này.
                      </div>
                    )}
                  </div>
                </div>
              )
            )
          ) : (
            <div className="alert alert-light border">
              Chưa có dữ liệu
              danh mục cẩm nang.
            </div>
          )}
        </main>

        <aside
          className="right-col flex-shrink-0"
          style={{
            width:
              '280px',
          }}
        >
          <div
            className="hotline-box mb-3 p-4 text-center text-white rounded-3 shadow-sm"
            style={{
              backgroundColor:
                'var(--primary, #0b63e5)',
            }}
          >
            <div className="fw-medium">
              Hotline hỗ trợ
            </div>

            <div
              className="phone fw-bold mt-1"
              style={{
                fontSize:
                  '22px',
              }}
            >
              1900 565 565
            </div>

            <div className="small opacity-75 mt-2">
              Tư vấn 24/7
            </div>
          </div>

          <div className="sidebar-ad rounded-3 overflow-hidden shadow-sm">
            <img
              src="https://nganngan5905-tech.github.io/LTWEB/10.jpg"
              alt="Bio Medic Center"
              className="w-100 d-block"
              style={{
                borderRadius:
                  '8px',
              }}
            />
          </div>
        </aside>
      </div>
    </div>
  );
}