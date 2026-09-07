import React, {
  useEffect,
  useMemo,
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getSocialPosts } from '../../services/newsService';
import { getApiErrorMessage } from '../../services/api';

const FALLBACK_POSTS = [
  {
    id: 1,
    title:
      'Bio Medic Center đồng hành cùng chương trình khám sức khỏe cộng đồng',
    summary:
      'Hoạt động tư vấn và kiểm tra sức khỏe giúp nâng cao nhận thức về phòng bệnh và khám sức khỏe định kỳ.',
    image:
      'https://placehold.co/900x520/eaf2ff/0360d9?text=Hoat+dong+cong+dong',
    createdAt: '05/09/2026',
  },
  {
    id: 2,
    title:
      'Chương trình hiến máu nhân đạo cùng đội ngũ Bio Medic Center',
    summary:
      'Đội ngũ nhân viên tham gia hoạt động hiến máu tình nguyện và lan tỏa tinh thần trách nhiệm với cộng đồng.',
    image:
      'https://placehold.co/900x520/eaf2ff/0360d9?text=Hien+mau',
    createdAt: '22/08/2026',
  },
  {
    id: 3,
    title:
      'Tư vấn sức khỏe miễn phí cho người cao tuổi',
    summary:
      'Chương trình hỗ trợ tư vấn sức khỏe, đo huyết áp và hướng dẫn theo dõi các yếu tố nguy cơ.',
    image:
      'https://placehold.co/900x520/eaf2ff/0360d9?text=Tu+van+suc+khoe',
    createdAt: '10/08/2026',
  },
];

const normalizePost = (item = {}, index = 0) => ({
  id:
    item.id ??
    item.idBaiViet ??
    item.IDBaiViet ??
    index,

  title:
    item.title ??
    item.tieuDe ??
    item.TieuDe ??
    'Bài viết',

  summary:
    item.summary ??
    item.description ??
    item.moTa ??
    item.MoTa ??
    '',

  image:
    item.image ??
    item.hinhAnh ??
    item.HinhAnh ??
    'https://placehold.co/900x520/eaf2ff/0360d9?text=Bio+Medic',

  createdAt:
    item.createdAt ??
    item.ngayDang ??
    item.NgayDang ??
    '',
});

export default function Social() {
  const [posts, setPosts] = useState(FALLBACK_POSTS);
  const [keyword, setKeyword] = useState('');

  const [loading, setLoading] = useState(true);

  const [message, setMessage] = useState({
    type: '',
    text: '',
  });

  useEffect(() => {
    let active = true;

    const loadPosts = async () => {
      try {
        const data =
          await getSocialPosts();

        if (!active) {
          return;
        }

        const list = Array.isArray(data)
          ? data
          : data?.content ||
            data?.items ||
            data?.data ||
            [];

        if (list.length > 0) {
          setPosts(
            list.map(normalizePost)
          );
        }
      } catch (err) {
        if (active) {
          setMessage({
            type: 'warning',
            text: getApiErrorMessage(
              err,
              'Chưa tải được bài viết từ backend. Hệ thống đang hiển thị nội dung mẫu.'
            ),
          });
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    loadPosts();

    return () => {
      active = false;
    };
  }, []);

  const filteredPosts =
    useMemo(() => {
      const q =
        keyword
          .trim()
          .toLowerCase();

      if (!q) {
        return posts;
      }

      return posts.filter((item) =>
        `${item.title} ${item.summary}`
          .toLowerCase()
          .includes(q)
      );
    }, [posts, keyword]);

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: 1120,
        }}
      >
        <div className="text-center mb-5">
          <h1
            className="fw-bold"
            style={{
              color: 'var(--primary)',
            }}
          >
            Hoạt động xã hội
          </h1>

          <p
            className="text-secondary mx-auto mb-0"
            style={{
              maxWidth: 760,
            }}
          >
            Những hoạt động cộng đồng, chương trình thiện nguyện và sự kiện xã hội của Bio Medic Center.
          </p>
        </div>

        {message.text && (
          <Notification
            type={message.type}
            message={message.text}
            onClose={() =>
              setMessage({
                type: '',
                text: '',
              })
            }
          />
        )}

        <div className="card border-0 shadow-sm rounded-4 mb-4">
          <div className="card-body p-4">
            <div className="input-group">
              <span className="input-group-text bg-white">
                <i className="fa-solid fa-magnifying-glass text-secondary" />
              </span>

              <input
                type="search"
                className="form-control"
                placeholder="Tìm hoạt động xã hội..."
                value={keyword}
                onChange={(e) =>
                  setKeyword(
                    e.target.value
                  )
                }
              />
            </div>
          </div>
        </div>

        {loading ? (
          <Loading text="Đang tải bài viết..." />
        ) : (
          <div className="row g-4">
            {filteredPosts.map((post, index) => (
              <div
                className="col-md-6"
                key={post.id ?? index}
              >
                <div className="card border-0 shadow-sm rounded-4 h-100 overflow-hidden">
                  <img
                    src={post.image}
                    alt={post.title}
                    className="card-img-top"
                    style={{
                      height: 250,
                      objectFit: 'cover',
                    }}
                  />

                  <div className="card-body p-4 d-flex flex-column">
                    {post.createdAt && (
                      <div className="small text-secondary mb-2">
                        <i className="fa-regular fa-calendar me-2" />
                        {post.createdAt}
                      </div>
                    )}

                    <h4 className="fw-bold">
                      {post.title}
                    </h4>

                    <p
                      className="text-secondary flex-grow-1"
                      style={{
                        lineHeight: 1.7,
                      }}
                    >
                      {post.summary}
                    </p>

                    <Link
                      to={`/hoat-dong-xa-hoi/${post.id}`}
                      className="btn btn-outline-primary align-self-start"
                    >
                      Đọc bài viết
                      <i className="fa-solid fa-arrow-right ms-2" />
                    </Link>
                  </div>
                </div>
              </div>
            ))}

            {filteredPosts.length === 0 && (
              <div className="col-12">
                <div className="alert alert-light border text-center">
                  Không tìm thấy hoạt động phù hợp.
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}