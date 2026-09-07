import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import {
  Link,
  useNavigate,
  useSearchParams,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { searchSite } from '../../services/searchService';
import { getApiErrorMessage } from '../../services/api';

const normalizeResult = (
  item = {},
  index = 0
) => ({
  id:
    item.id ??
    item.resultId ??
    index,

  type:
    item.type ??
    item.loai ??
    'OTHER',

  title:
    item.title ??
    item.name ??
    item.tieuDe ??
    item.ten ??
    'Kết quả tìm kiếm',

  description:
    item.description ??
    item.summary ??
    item.moTa ??
    '',

  url:
    item.url ??
    item.route ??
    '/',

  image:
    item.image ??
    item.hinhAnh ??
    '',

  meta:
    item.meta ??
    item.category ??
    item.nhom ??
    '',
});

const typeLabel = (type) => {
  switch (
    String(type || '')
      .toUpperCase()
  ) {
    case 'TEST':
      return 'Xét nghiệm';

    case 'DOCTOR':
      return 'Bác sĩ';

    case 'MEDICAL':
    case 'ARTICLE':
      return 'Bài viết';

    case 'PROMOTION':
      return 'Ưu đãi';

    case 'SOCIAL':
      return 'Hoạt động xã hội';

    default:
      return 'Khác';
  }
};

export default function TimKiem() {
  const navigate =
    useNavigate();

  const [
    searchParams,
  ] = useSearchParams();

  const queryFromUrl =
    searchParams.get('q') ??
    '';

  const [keyword, setKeyword] =
    useState(queryFromUrl);

  const [results, setResults] =
    useState([]);

  const [
    selectedType,
    setSelectedType,
  ] = useState('ALL');

  const [loading, setLoading] =
    useState(false);

  const [error, setError] =
    useState('');

  const runSearch = async (
    query
  ) => {
    const q =
      String(query || '')
        .trim();

    if (!q) {
      setResults([]);
      return;
    }

    try {
      setLoading(true);
      setError('');

      const data =
        await searchSite(q);

      const list =
        Array.isArray(data)
          ? data
          : data?.content ||
            data?.items ||
            data?.results ||
            data?.data ||
            [];

      setResults(
        list.map(
          normalizeResult
        )
      );
    } catch (err) {
      setError(
        getApiErrorMessage(
          err,
          'Không thể thực hiện tìm kiếm.'
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    setKeyword(
      queryFromUrl
    );

    runSearch(
      queryFromUrl
    );
  }, [queryFromUrl]);

  const handleSubmit =
    (e) => {
      e.preventDefault();

      const q =
        keyword.trim();

      if (!q) {
        return;
      }

      navigate(
        `/tim-kiem?q=${encodeURIComponent(
          q
        )}`
      );
    };

  const types =
    useMemo(() => {
      return [
        'ALL',
        ...new Set(
          results.map(
            (item) =>
              String(
                item.type ||
                  'OTHER'
              ).toUpperCase()
          )
        ),
      ];
    }, [results]);

  const visibleResults =
    useMemo(() => {
      if (
        selectedType ===
        'ALL'
      ) {
        return results;
      }

      return results.filter(
        (item) =>
          String(
            item.type ||
              'OTHER'
          ).toUpperCase() ===
          selectedType
      );
    }, [
      results,
      selectedType,
    ]);

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: 1050,
        }}
      >
        <h1
          className="fw-bold mb-4 text-center"
          style={{
            color:
              'var(--primary)',
          }}
        >
          Tìm kiếm
        </h1>

        {error && (
          <Notification
            type="danger"
            message={error}
            onClose={() =>
              setError('')
            }
          />
        )}

        <div className="card border-0 shadow-sm rounded-4 mb-4">
          <div className="card-body p-4">
            <form
              className="input-group input-group-lg"
              onSubmit={
                handleSubmit
              }
            >
              <span className="input-group-text bg-white">
                <i className="fa-solid fa-magnifying-glass text-secondary" />
              </span>

              <input
                type="search"
                className="form-control"
                placeholder="Tìm xét nghiệm, bác sĩ, bài viết, khuyến mãi..."
                value={keyword}
                onChange={(e) =>
                  setKeyword(
                    e.target.value
                  )
                }
              />

              <button className="btn btn-primary px-4">
                Tìm kiếm
              </button>
            </form>
          </div>
        </div>

        {loading ? (
          <Loading text="Đang tìm kiếm..." />
        ) : queryFromUrl ? (
          <>
            <div className="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
              <div className="text-secondary">
                Tìm thấy{' '}
                <strong>
                  {
                    results.length
                  }
                </strong>{' '}
                kết quả cho{' '}
                <strong>
                  “
                  {
                    queryFromUrl
                  }
                  ”
                </strong>
              </div>

              <div className="d-flex flex-wrap gap-2">
                {types.map(
                  (type) => (
                    <button
                      type="button"
                      key={type}
                      className={`btn btn-sm ${
                        selectedType ===
                        type
                          ? 'btn-primary'
                          : 'btn-outline-primary'
                      }`}
                      onClick={() =>
                        setSelectedType(
                          type
                        )
                      }
                    >
                      {type ===
                      'ALL'
                        ? 'Tất cả'
                        : typeLabel(
                            type
                          )}
                    </button>
                  )
                )}
              </div>
            </div>

            <div className="d-flex flex-column gap-3">
              {visibleResults.map(
                (
                  item,
                  index
                ) => (
                  <Link
                    key={
                      item.id ??
                      index
                    }
                    to={
                      item.url
                    }
                    className="card border-0 shadow-sm rounded-4 text-decoration-none text-dark"
                  >
                    <div className="card-body p-4">
                      <div className="row g-3 align-items-center">
                        {item.image && (
                          <div className="col-md-3">
                            <img
                              src={
                                item.image
                              }
                              alt={
                                item.title
                              }
                              className="img-fluid rounded-3 w-100"
                              style={{
                                height:
                                  125,
                                objectFit:
                                  'cover',
                              }}
                            />
                          </div>
                        )}

                        <div
                          className={
                            item.image
                              ? 'col-md-9'
                              : 'col-12'
                          }
                        >
                          <div className="d-flex flex-wrap align-items-center gap-2 mb-2">
                            <span className="badge bg-primary-subtle text-primary">
                              {typeLabel(
                                item.type
                              )}
                            </span>

                            {item.meta && (
                              <span className="small text-secondary">
                                {
                                  item.meta
                                }
                              </span>
                            )}
                          </div>

                          <h5 className="fw-bold mb-2">
                            {
                              item.title
                            }
                          </h5>

                          <p className="text-secondary mb-0">
                            {
                              item.description
                            }
                          </p>
                        </div>
                      </div>
                    </div>
                  </Link>
                )
              )}

              {visibleResults.length ===
                0 && (
                <div className="alert alert-light border text-center">
                  Không tìm thấy kết quả phù hợp.
                </div>
              )}
            </div>
          </>
        ) : (
          <div className="alert alert-light border text-center">
            Nhập từ khóa để bắt đầu tìm kiếm.
          </div>
        )}
      </div>
    </div>
  );
}