import React, {
  useEffect,
  useState,
} from 'react';

import {
  Link,
  useSearchParams,
} from 'react-router-dom';

import {
  getMedicalArticles,
  getMedicalErrorMessage,
} from '../../services/medicalService';

const formatDate = (dateStr) => {
  if (!dateStr) {
    return '—';
  }

  const d =
    new Date(dateStr);

  if (
    Number.isNaN(
      d.getTime()
    )
  ) {
    return dateStr;
  }

  const day =
    String(
      d.getDate()
    ).padStart(2, '0');

  const month =
    String(
      d.getMonth() + 1
    ).padStart(2, '0');

  const year =
    d.getFullYear();

  return `${day}/${month}/${year}`;
};

const safeImage = (src) =>
  src ||
  'https://placehold.co/400x300/0B63E5/FFFFFF/png?text=Y+Khoa';

const normalizeArticle = (
  item = {}
) => ({
  id:
    item.id ??
    item.idBaiViet ??
    item.IDBaiViet ??
    item.idTinTuc ??
    item.IDTinTuc,

  title:
    item.title ??
    item.tieuDe ??
    item.TieuDe ??
    '',

  content:
    item.content ??
    item.noiDung ??
    item.NoiDung ??
    item.moTa ??
    item.MoTa ??
    '',

  image:
    item.image ??
    item.hinhAnh ??
    item.HinhAnh ??
    '',

  createdAt:
    item.createdAt ??
    item.created_at ??
    item.ngayDang ??
    item.NgayDang ??
    '',
});

export default function Medical() {
  const [
    searchParams,
    setSearchParams,
  ] = useSearchParams();

  const initialPage =
    Math.max(
      Number(
        searchParams.get(
          'page'
        )
      ) || 1,
      1
    );

  const [articles, setArticles] =
    useState([]);

  const [featured, setFeatured] =
    useState(null);

  const [latest, setLatest] =
    useState([]);

  const [page, setPage] =
    useState(initialPage);

  const [
    totalPages,
    setTotalPages,
  ] = useState(1);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState('');

  useEffect(() => {
    let active = true;

    const loadArticles =
      async () => {
        try {
          setLoading(true);
          setError('');

          const data =
            await getMedicalArticles(
              page,
              8
            );

          if (!active) {
            return;
          }

          /*
           * Hỗ trợ cả response cũ:
           * { articles, featured, latest, totalPages }
           *
           * và Spring Page:
           * { content, totalPages }
           */
          const rawArticles =
            data?.articles ??
            data?.content ??
            (Array.isArray(data)
              ? data
              : []);

          const articleList =
            rawArticles.map(
              normalizeArticle
            );

          const rawFeatured =
            data?.featured;

          const rawLatest =
            data?.latest ??
            [];

          setArticles(
            articleList
          );

          setFeatured(
            rawFeatured
              ? normalizeArticle(
                  rawFeatured
                )
              : articleList[0] ??
                  null
          );

          setLatest(
            rawLatest.length >
              0
              ? rawLatest.map(
                  normalizeArticle
                )
              : articleList.slice(
                  0,
                  5
                )
          );

          setTotalPages(
            Math.max(
              Number(
                data?.totalPages
              ) || 1,
              1
            )
          );
        } catch (err) {
          if (!active) {
            return;
          }

          setArticles([]);
          setFeatured(null);
          setLatest([]);

          setError(
            getMedicalErrorMessage(
              err,
              'Không thể tải kiến thức y khoa.'
            )
          );
        } finally {
          if (active) {
            setLoading(false);
          }
        }
      };

    loadArticles();

    return () => {
      active = false;
    };
  }, [page]);

  const handlePageChange = (
    newPage
  ) => {
    if (
      newPage < 1 ||
      newPage >
        totalPages ||
      newPage === page
    ) {
      return;
    }

    setPage(newPage);

    setSearchParams({
      page: String(
        newPage
      ),
    });

    window.scrollTo({
      top: 0,
      behavior: 'smooth',
    });
  };

  const renderPaginationItems =
    () => {
      const visible = 7;

      let start =
        Math.max(
          1,
          page - 3
        );

      let end =
        Math.min(
          totalPages,
          start +
            visible -
            1
        );

      if (
        end -
          start +
          1 <
        visible
      ) {
        start =
          Math.max(
            1,
            end -
              visible +
              1
          );
      }

      const items = [];

      if (start > 1) {
        items.push(
          <div
            key="page-1"
            className="page-item"
            style={{
              margin:
                '0 3px',
            }}
          >
            <button
              type="button"
              className="btn btn-sm btn-outline-secondary"
              onClick={() =>
                handlePageChange(
                  1
                )
              }
            >
              1
            </button>
          </div>
        );

        if (start > 2) {
          items.push(
            <div
              key="dots-start"
              className="page-item"
              style={{
                margin:
                  '0 3px',
              }}
            >
              <span className="btn btn-sm btn-light disabled">
                ...
              </span>
            </div>
          );
        }
      }

      for (
        let p = start;
        p <= end;
        p++
      ) {
        items.push(
          <div
            key={p}
            className={`page-item ${
              p === page
                ? 'active'
                : ''
            }`}
            style={{
              margin:
                '0 3px',
            }}
          >
            <button
              type="button"
              className={`btn btn-sm ${
                p === page
                  ? 'btn-primary'
                  : 'btn-outline-secondary'
              }`}
              style={
                p === page
                  ? {
                      backgroundColor:
                        'var(--primary, #0b63e5)',

                      borderColor:
                        'var(--primary, #0b63e5)',
                    }
                  : {}
              }
              onClick={() =>
                handlePageChange(
                  p
                )
              }
            >
              {p}
            </button>
          </div>
        );
      }

      if (
        end <
        totalPages
      ) {
        if (
          end <
          totalPages - 1
        ) {
          items.push(
            <div
              key="dots-end"
              className="page-item"
              style={{
                margin:
                  '0 3px',
              }}
            >
              <span className="btn btn-sm btn-light disabled">
                ...
              </span>
            </div>
          );
        }

        items.push(
          <div
            key="last-page"
            className="page-item"
            style={{
              margin:
                '0 3px',
            }}
          >
            <button
              type="button"
              className="btn btn-sm btn-outline-secondary"
              onClick={() =>
                handlePageChange(
                  totalPages
                )
              }
            >
              {
                totalPages
              }
            </button>
          </div>
        );
      }

      return items;
    };

  return (
    <div
      className="container my-4"
      style={{
        maxWidth:
          '1200px',
      }}
    >
      <div
        className="section-title text-start fw-bold mb-4"
        style={{
          fontSize:
            '32px',

          color:
            'var(--primary, #0b63e5)',
        }}
      >
        KIẾN THỨC Y KHOA
      </div>

      {error && (
        <div className="alert alert-danger">
          {error}
        </div>
      )}

      <div className="row g-4">
        {/* =========================
            MAIN
        ========================= */}

        <main className="col-lg-8">
          {loading ? (
            <div className="text-center py-5 text-secondary">
              <div
                className="spinner-border text-primary mb-2"
                role="status"
              />

              <div>
                Đang tải kiến thức
                y khoa...
              </div>
            </div>
          ) : (
            <>
              {featured && (
                <div className="card border-0 shadow-sm mb-4 rounded-3 overflow-hidden">
                  <div className="row g-0">
                    <div className="col-md-6">
                      <Link
                        to={`/medical/${featured.id}`}
                      >
                        <img
                          src={safeImage(
                            featured.image
                          )}
                          alt={
                            featured.title
                          }
                          className="w-100 h-100"
                          style={{
                            minHeight:
                              '220px',

                            maxHeight:
                              '350px',

                            objectFit:
                              'cover',
                          }}
                        />
                      </Link>
                    </div>

                    <div className="col-md-6 p-3 d-flex flex-column justify-content-between">
                      <div>
                        <div
                          className="text-uppercase fw-semibold mb-1"
                          style={{
                            color:
                              '#9aa3ab',

                            fontSize:
                              '13px',

                            letterSpacing:
                              '1px',
                          }}
                        >
                          Tin tức hot nhất
                        </div>

                        <h4 className="fw-bold mb-2">
                          <Link
                            to={`/medical/${featured.id}`}
                            className="text-decoration-none"
                            style={{
                              color:
                                'var(--primary, #0b63e5)',
                            }}
                          >
                            {
                              featured.title
                            }
                          </Link>
                        </h4>

                        <p
                          className="text-muted small mb-3"
                          style={{
                            lineHeight:
                              '1.6',
                          }}
                        >
                          {featured.content
                            ? `${featured.content.substring(
                                0,
                                320
                              )}${
                                featured.content
                                  .length >
                                320
                                  ? '...'
                                  : ''
                              }`
                            : ''}
                        </p>
                      </div>

                      <div className="text-secondary small">
                        {formatDate(
                          featured.createdAt
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              )}

              {articles.length >
              0 ? (
                articles.map(
                  (item) => (
                    <div
                      key={
                        item.id
                      }
                      className="card border-0 shadow-sm mb-3 rounded-3 overflow-hidden"
                    >
                      <div className="row g-0 align-items-center">
                        <div className="col-md-4">
                          <Link
                            to={`/medical/${item.id}`}
                          >
                            <img
                              src={safeImage(
                                item.image
                              )}
                              alt={
                                item.title
                              }
                              className="w-100"
                              style={{
                                height:
                                  '140px',

                                objectFit:
                                  'cover',
                              }}
                            />
                          </Link>
                        </div>

                        <div className="col-md-8 p-3">
                          <div className="text-secondary small mb-1">
                            {formatDate(
                              item.createdAt
                            )}
                          </div>

                          <h5 className="fw-bold mb-1">
                            <Link
                              to={`/medical/${item.id}`}
                              className="text-decoration-none"
                              style={{
                                color:
                                  'var(--primary, #0b63e5)',
                              }}
                            >
                              {
                                item.title
                              }
                            </Link>
                          </h5>

                          <p
                            className="text-muted small mb-0"
                            style={{
                              lineHeight:
                                '1.5',
                            }}
                          >
                            {item.content
                              ? `${item.content.substring(
                                  0,
                                  200
                                )}${
                                  item.content
                                    .length >
                                  200
                                    ? '...'
                                    : ''
                                }`
                              : ''}
                          </p>
                        </div>
                      </div>
                    </div>
                  )
                )
              ) : (
                <p className="text-muted">
                  Chưa có bài viết.
                </p>
              )}

              {totalPages > 1 && (
                <nav
                  className="d-flex justify-content-center my-4"
                  aria-label="Page navigation"
                >
                  <div
                    className="page-item"
                    style={{
                      margin:
                        '0 3px',
                    }}
                  >
                    <button
                      type="button"
                      className="btn btn-sm btn-outline-secondary"
                      disabled={
                        page <= 1
                      }
                      onClick={() =>
                        handlePageChange(
                          page - 1
                        )
                      }
                    >
                      &laquo;
                    </button>
                  </div>

                  {renderPaginationItems()}

                  <div
                    className="page-item"
                    style={{
                      margin:
                        '0 3px',
                    }}
                  >
                    <button
                      type="button"
                      className="btn btn-sm btn-outline-secondary"
                      disabled={
                        page >=
                        totalPages
                      }
                      onClick={() =>
                        handlePageChange(
                          page + 1
                        )
                      }
                    >
                      &raquo;
                    </button>
                  </div>
                </nav>
              )}
            </>
          )}
        </main>

        {/* =========================
            SIDEBAR
        ========================= */}

        <aside className="col-lg-4">
          <div
            className="p-4 text-center text-white rounded-3 mb-4 shadow-sm"
            style={{
              backgroundColor:
                'var(--primary, #0b63e5)',
            }}
          >
            <div className="fw-medium">
              Hotline hỗ trợ
            </div>

            <div className="fs-4 fw-bold mt-1">
              1900 565 565
            </div>

            <div className="small opacity-75 mt-1">
              Tư vấn sức khỏe
              24/7
            </div>
          </div>

          <div className="mb-4 rounded-3 overflow-hidden shadow-sm">
            <img
              src="https://nganngan5905-tech.github.io/LTWEB/11.jpg"
              alt="Bio Medic Center"
              className="w-100 d-block"
            />
          </div>

          <div className="card border-0 shadow-sm p-3 rounded-3">
            <h5 className="fw-bold mb-3">
              Tin tức mới nhất
            </h5>

            {latest.length >
            0 ? (
              latest.map(
                (lt) => (
                  <div
                    key={
                      lt.id
                    }
                    className="d-flex gap-2 mb-3 align-items-center"
                  >
                    <Link
                      to={`/medical/${lt.id}`}
                      className="flex-shrink-0"
                    >
                      <img
                        src={safeImage(
                          lt.image
                        )}
                        alt={
                          lt.title
                        }
                        style={{
                          width:
                            '84px',

                          height:
                            '64px',

                          objectFit:
                            'cover',

                          borderRadius:
                            '6px',
                        }}
                      />
                    </Link>

                    <div>
                      <Link
                        to={`/medical/${lt.id}`}
                        className="text-decoration-none fw-semibold small d-block mb-1"
                        style={{
                          color:
                            'var(--primary, #0b63e5)',
                        }}
                      >
                        {lt.title
                          ? lt.title
                              .length >
                            60
                            ? `${lt.title.substring(
                                0,
                                60
                              )}...`
                            : lt.title
                          : ''}
                      </Link>

                      <div
                        className="text-secondary"
                        style={{
                          fontSize:
                            '12px',
                        }}
                      >
                        {formatDate(
                          lt.createdAt
                        )}
                      </div>
                    </div>
                  </div>
                )
              )
            ) : (
              <div className="text-muted small">
                Chưa có bài viết
                mới.
              </div>
            )}
          </div>
        </aside>
      </div>
    </div>
  );
}